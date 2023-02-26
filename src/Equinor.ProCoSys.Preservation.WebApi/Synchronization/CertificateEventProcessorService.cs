using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Equinor.ProCoSys.Preservation.Command.TagCommands.AutoTransfer;
using Equinor.ProCoSys.Preservation.WebApi.Authentication;
using Equinor.ProCoSys.Preservation.WebApi.Telemetry;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceResult;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Auth.Authentication;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class CertificateEventProcessorService : ICertificateEventProcessorService
    {
        private readonly Guid _preservationApiOid;

        private readonly ILogger _logger;
        private readonly ITelemetryClient _telemetryClient;
        private readonly IMediator _mediator;
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;
        private readonly IPlantSetter _plantSetter;
        private readonly ICurrentUserSetter _currentUserSetter;
        private readonly IClaimsTransformation _claimsTransformation;
        private readonly IMainApiAuthenticator _mainApiAuthenticator;

        private const string PreservationBusReceiverTelemetryEvent = "Preservation Bus Receiver";

        public CertificateEventProcessorService(
            ILogger<CertificateEventProcessorService> logger,
            ITelemetryClient telemetryClient,
            IMediator mediator,
            IClaimsPrincipalProvider claimsPrincipalProvider,
            IPlantSetter plantSetter,
            ICurrentUserSetter currentUserSetter,
            IClaimsTransformation claimsTransformation,
            IMainApiAuthenticator mainApiAuthenticator,
            IOptionsSnapshot<PreservationAuthenticatorOptions> authenticatorOptions
        )
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _mediator = mediator;
            _claimsPrincipalProvider = claimsPrincipalProvider;
            _currentUserSetter = currentUserSetter;
            _claimsTransformation = claimsTransformation;
            _plantSetter = plantSetter;
            _mainApiAuthenticator = mainApiAuthenticator;
            _preservationApiOid = authenticatorOptions.Value.PreservationApiObjectId;
        }

        //TODO: ADD TESTS FOR THIS CLASS

        public async Task ProcessCertificateEventAsync(string messageJson)
        {
            var certificateEvent = JsonSerializer.Deserialize<CertificateTopic>(messageJson);
            if (certificateEvent != null && certificateEvent.Behavior == "delete")
            {
                TrackUnsupportedDeleteEvent(PcsTopic.Certificate, certificateEvent.ProCoSysGuid);
                return;
            }

            if (certificateEvent != null && (
                certificateEvent.Plant.IsEmpty() ||
                certificateEvent.CertificateNo.IsEmpty()))
            {
                throw new ArgumentNullException($"Deserialized JSON is not a valid CertificateEvent {messageJson}");
            }

            TrackCertificateEvent(certificateEvent);

            await HandleAutoTransferIfRelevantAsync(certificateEvent);
        }

        private async Task HandleAutoTransferIfRelevantAsync(CertificateTopic certificateEvent)
        {
            if (certificateEvent.CertificateType == "RFOC" || certificateEvent.CertificateType == "RFCC")
            {
                await SetUserContextAsync(certificateEvent.Plant);
                var result = await _mediator.Send(new AutoTransferCommand(
                    certificateEvent.ProjectName,
                    certificateEvent.CertificateNo,
                    certificateEvent.CertificateType.ToString(),
                    certificateEvent.ProCoSysGuid));

                LogAutoTransferResult(certificateEvent, result);
            }
        }

        private async Task SetUserContextAsync(string plant)
        {
            _mainApiAuthenticator.AuthenticationType = AuthenticationType.AsApplication;
            _currentUserSetter.SetCurrentUserOid(_preservationApiOid);
            var currentUser = _claimsPrincipalProvider.GetCurrentClaimsPrincipal();

            _plantSetter.SetPlant(plant);
            await _claimsTransformation.TransformAsync(currentUser);
        }

        private void LogAutoTransferResult(CertificateTopic certificateEvent, Result<Unit> result)
        {
            var resultOk = result.ResultType == ResultType.Ok;

            _logger.LogInformation(resultOk ? "Autotransfer tags complete." : "Autotransfer tags functions failed.");

            var telemetryDictionary = new Dictionary<string, string>
            {
                {"Status", resultOk ? "Succeeded" : "Failed"},
                {"Plant", certificateEvent.Plant},
                {"Type", "Autotransfer tags"},
                {"ProjectName", String.IsNullOrWhiteSpace(certificateEvent.ProjectName) ? "null or empty" : certificateEvent.ProjectName},
                {"CertificateNo", certificateEvent.CertificateNo},
                {"CertificateType", certificateEvent.CertificateType}
            };

            _telemetryClient.TrackEvent("Synchronization Status", telemetryDictionary);
        }

        private void TrackCertificateEvent(CertificateTopic certificateTopic) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event", PcsServiceBus.Topics.CertificateTopic.TopicName},
                    {nameof(certificateTopic.CertificateNo), certificateTopic.CertificateNo},
                    {nameof(certificateTopic.CertificateType), certificateTopic.CertificateType},
                    {nameof(certificateTopic.CertificateStatus), certificateTopic.CertificateStatus.ToString()},
                    {nameof(certificateTopic.Plant), NormalizePlant(certificateTopic.Plant)},
                    {nameof(certificateTopic.ProjectName), NormalizeProjectName(certificateTopic.ProjectName)}
                });

        private void TrackUnsupportedDeleteEvent(PcsTopic topic, Guid guid) =>
            _telemetryClient.TrackEvent(PreservationBusReceiverTelemetryEvent,
                new Dictionary<string, string>
                {
                    {"Event Delete", topic.ToString()},
                    {"ProCoSysGuid", guid.ToString()},
                    {"Supported", "false"}
                });

        private string NormalizePlant(string plant) => plant[4..];

        private string NormalizeProjectName(string projectName) => String.IsNullOrWhiteSpace(projectName) ? null : projectName.Replace('$', '_');
    }
}
