using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.MiscCommands.UpdateDateTimeSetting;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncProjects;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncResponsibles;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncTagFunctions;
using Equinor.Procosys.Preservation.Command.TagCommands.AutoTransfer;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.SettingAggregate;
using Equinor.Procosys.Preservation.Domain.Time;
using Equinor.Procosys.Preservation.MainApi.Certificate;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.Query.GetDateTimeSetting;
using Equinor.Procosys.Preservation.WebApi.Authentication;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Equinor.Procosys.Preservation.WebApi.Telemetry;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly Guid _synchronizationUserOid;
        private readonly ILogger<SynchronizationService> _logger;
        private readonly ITelemetryClient _telemetryClient;
        private readonly IMediator _mediator;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IPlantSetter _plantSetter;
        private readonly ICurrentUserSetter _currentUserSetter;
        private readonly IBearerTokenSetter _bearerTokenSetter;
        private readonly IClaimsTransformation _claimsTransformation;
        private readonly IApplicationAuthenticator _authenticator;
        private readonly IPlantCache _plantCache;
        private readonly IOptionsMonitor<SynchronizationOptions> _options;
        private readonly ICertificateApiService _certificateApiService;

        public SynchronizationService(
            ILogger<SynchronizationService> logger,
            ITelemetryClient telemetryClient,
            IMediator mediator,
            IClaimsProvider claimsProvider,
            IPlantSetter plantSetter,
            ICurrentUserSetter currentUserSetter,
            IBearerTokenSetter bearerTokenSetter,
            IClaimsTransformation claimsTransformation,
            IApplicationAuthenticator authenticator,
            IPlantCache plantCache,
            IOptionsMonitor<SynchronizationOptions> options,
            ICertificateApiService certificateApiService)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _mediator = mediator;
            _claimsProvider = claimsProvider;
            _currentUserSetter = currentUserSetter;
            _claimsTransformation = claimsTransformation;
            _plantSetter = plantSetter;
            _authenticator = authenticator;
            _bearerTokenSetter = bearerTokenSetter;
            _plantCache = plantCache;
            _options = options;
            _certificateApiService = certificateApiService;
            _synchronizationUserOid = options.CurrentValue.UserOid;
        }

        public async Task Synchronize(CancellationToken cancellationToken)
        {
            var bearerToken = await _authenticator.GetBearerTokenForApplicationAsync();
            _bearerTokenSetter.SetBearerToken(bearerToken, false);

            _currentUserSetter.SetCurrentUser(_synchronizationUserOid);

            var currentUser = _claimsProvider.GetCurrentUser();
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimsExtensions.OidType, _synchronizationUserOid.ToString()));
            currentUser.AddIdentity(claimsIdentity);

            foreach (var plant in await _plantCache.GetPlantIdsForUserOidAsync(_synchronizationUserOid))
            {
                _logger.LogInformation($"Synchronizing plant {plant}...");

                _plantSetter.SetPlant(plant);
                await _claimsTransformation.TransformAsync(currentUser);

                var startTime = TimeService.UtcNow;
                if (_options.CurrentValue.AutoTransferTags)
                {
                    await AutoTransferTagsAsync(plant);
                }

                if (_options.CurrentValue.SynchronizeProjects)
                {
                    await SynchronizeProjectsAsync(plant);
                }

                if (_options.CurrentValue.SynchronizeResponsibles)
                {
                    await SynchronizeResponsiblesAsync(plant);
                }

                if (_options.CurrentValue.SynchronizeTagFunctions)
                {
                    await SynchronizeTagFunctionsAsync(plant);
                }
                
                var endTime = TimeService.UtcNow;

                _logger.LogInformation($"Plant {plant} synchronized. Duration: {(endTime - startTime).TotalSeconds}s.");
                _telemetryClient.TrackMetric("Synchronization Time", (endTime - startTime).TotalSeconds, "Plant", plant);
            }
        }

        private async Task SynchronizeProjectsAsync(string plant)
        {
            _logger.LogInformation("Synchronizing projects");

            var result = await _mediator.Send(new SyncProjectsCommand());

            if (result.ResultType == ServiceResult.ResultType.Ok)
            {
                _logger.LogInformation("Synchronizing projects complete.");
                _telemetryClient.TrackEvent("Synchronization Status",
                    new Dictionary<string, string>
                    {
                        {"Status", "Succeeded"}, 
                        {"Plant", plant},
                        {"Type", "Projects"}
                    });
            }
            else
            {
                _logger.LogWarning($"Synchronizing projects failed. ResultType {result.ResultType}");
                _telemetryClient.TrackEvent("Synchronization Status",
                    new Dictionary<string, string>
                    {
                        {"Status", "Failed"},
                        {"Plant", plant},
                        {"Type", "Projects"},
                        {"ResultType", result.ResultType.ToString()}
                    });
            }
        }

        private async Task SynchronizeResponsiblesAsync(string plant)
        {
            _logger.LogInformation("Synchronizing responsibles");

            var result = await _mediator.Send(new SyncResponsiblesCommand());

            if (result.ResultType == ServiceResult.ResultType.Ok)
            {
                _logger.LogInformation("Synchronizing responsibles complete.");
                _telemetryClient.TrackEvent("Synchronization Status",
                    new Dictionary<string, string>
                    {
                        {"Status", "Succeeded"},
                        {"Plant", plant},
                        {"Type", "Responsibles"}
                    });
            }
            else
            {
                _logger.LogWarning($"Synchronizing responsibles failed. ResultType {result.ResultType}");
                _telemetryClient.TrackEvent("Synchronization Status",
                    new Dictionary<string, string>
                    {
                        {"Status", "Failed"}, 
                        {"Plant", plant}, 
                        {"Type", "Responsibles"},
                        {"ResultType", result.ResultType.ToString()}
                    });
            }
        }

        private async Task SynchronizeTagFunctionsAsync(string plant)
        {
            _logger.LogInformation("Synchronizing tag functions");

            var result = await _mediator.Send(new SyncTagFunctionsCommand());

            if (result.ResultType == ServiceResult.ResultType.Ok)
            {
                _logger.LogInformation("Synchronizing tag functions complete.");
                _telemetryClient.TrackEvent("Synchronization Status",
                    new Dictionary<string, string>
                    {
                        {"Status", "Succeeded"},
                        {"Plant", plant}, 
                        {"Type", "Tag Functions"}
                    });
            }
            else
            {
                _logger.LogWarning($"Synchronizing tag functions failed. ResultType {result.ResultType}");
                _telemetryClient.TrackEvent("Synchronization Status",
                    new Dictionary<string, string>
                    {
                        {"Status", "Failed"},
                        {"Plant", plant},
                        {"Type", "Tag Functions"},
                        {"ResultType", result.ResultType.ToString()}
                    });
            }
        }

        private async Task AutoTransferTagsAsync(string plant)
        {
            _logger.LogInformation("Autotransfer tags");

            var lastAcceptedCertificatesRead = await GetLastAcceptedCertificatesRead(plant);
            if (!lastAcceptedCertificatesRead.HasValue)
            {
                return;
            }

            var acceptedCertificatesSinceLastTransfer = (await _certificateApiService.GetAcceptedCertificatesAsync(plant, lastAcceptedCertificatesRead.Value)).ToList();

            // RFCC must be handled before RFOC, since RFCC is Accepted before RFOC. It is possible the same tag should be transfered 2 steps 
            await AutoTransferTagAffectedByCertificatesAsync(plant, acceptedCertificatesSinceLastTransfer.Where(c => c.CertificateType == "RFCC").ToList());
            await AutoTransferTagAffectedByCertificatesAsync(plant, acceptedCertificatesSinceLastTransfer.Where(c => c.CertificateType == "RFOC").ToList());

            await UpdateLastAcceptedCertificatesRead(plant);
        }

        private async Task UpdateLastAcceptedCertificatesRead(string plant)
        {
            var result = await _mediator.Send(new UpdateDateTimeSettingCommand(Setting.LastAcceptedCertificatesReadCode, TimeService.UtcNow));

            if (result.ResultType != ServiceResult.ResultType.Ok)
            {
                _logger.LogWarning(
                    $"Autotransfer tags functions failed. Could not update {Setting.LastAcceptedCertificatesReadCode}. ResultType {result.ResultType}");
                _telemetryClient.TrackEvent("Synchronization Status",
                    new Dictionary<string, string>
                    {
                        {"Status", "Failed"},
                        {"Plant", plant},
                        {"Type", "Autotransfer tags"},
                        {"ResultType", result.ResultType.ToString()}
                    });
            }
        }

        private async Task<DateTime?> GetLastAcceptedCertificatesRead(string plant)
        {
            var dateTimeResult =
                await _mediator.Send(new GetDateTimeSettingQuery(Setting.LastAcceptedCertificatesReadCode));
            if (dateTimeResult.ResultType == ServiceResult.ResultType.Ok && dateTimeResult.Data.HasValue)
            {
                return dateTimeResult.Data.Value;
            }

            if (dateTimeResult.ResultType == ServiceResult.ResultType.NotFound
                || (dateTimeResult.ResultType == ServiceResult.ResultType.Ok && !dateTimeResult.Data.HasValue))
            {
                return TimeService.UtcNow;
            }

            _logger.LogWarning(
                $"Autotransfer tags functions failed. Could not get {Setting.LastAcceptedCertificatesReadCode}. ResultType {dateTimeResult.ResultType}");
            _telemetryClient.TrackEvent("Synchronization Status",
                new Dictionary<string, string>
                {
                    {"Status", "Failed"},
                    {"Plant", plant},
                    {"Type", "Autotransfer tags"},
                    {"ResultType", dateTimeResult.ResultType.ToString()}
                });
            return null;
        }

        private async Task AutoTransferTagAffectedByCertificatesAsync(string plant, List<ProcosysCertificateModel> certificates)
        {
            foreach (var certificate in certificates)
            {
                var result = await _mediator.Send(new AutoTransferCommand(certificate.ProjectName, certificate.CertificateNo, certificate.CertificateType));

                if (result.ResultType == ServiceResult.ResultType.Ok)
                {
                    _logger.LogInformation("Autotransfer tags complete.");
                    _telemetryClient.TrackEvent("Synchronization Status",
                        new Dictionary<string, string>
                        {
                            {"Status", "Succeeded"}, 
                            {"Plant", plant}, 
                            {"Type", "Autotransfer tags"}, 
                            {"ProjectName", certificate.ProjectName}, 
                            {"CertificateNo", certificate.CertificateNo}, 
                            {"CertificateType", certificate.CertificateType}
                        });
                }
                else
                {
                    _logger.LogWarning($"Autotransfer tags functions failed.");
                    _telemetryClient.TrackEvent("Synchronization Status",
                        new Dictionary<string, string>
                        {
                            {"Status", "Failed"}, 
                            {"Plant", plant}, 
                            {"Type", "Autotransfer tags"}, 
                            {"ProjectName", certificate.ProjectName}, 
                            {"CertificateNo", certificate.CertificateNo}, 
                            {"CertificateType", certificate.CertificateType}
                        });
                }
            }
        }
    }
}
