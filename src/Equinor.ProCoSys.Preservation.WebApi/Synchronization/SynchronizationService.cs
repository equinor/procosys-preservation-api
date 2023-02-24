using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.SyncTagData;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.SettingAggregate;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using Equinor.ProCoSys.Preservation.WebApi.Authentication;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly Guid _preservationApiOid;
        private readonly ILogger<SynchronizationService> _logger;
        private readonly IMediator _mediator;
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;
        private readonly IPlantSetter _plantSetter;
        private readonly ICurrentUserSetter _currentUserSetter;
        private readonly IClaimsTransformation _claimsTransformation;
        private readonly IAuthenticator _authenticator;
        private readonly IPlantCache _plantCache;
        private ISettingRepository _settingRepository;
        private string _machine;

        public SynchronizationService(
            ILogger<SynchronizationService> logger,
            IMediator mediator,
            IClaimsPrincipalProvider claimsPrincipalProvider,
            IPlantSetter plantSetter,
            ICurrentUserSetter currentUserSetter,
            IClaimsTransformation claimsTransformation,
            IAuthenticator authenticator,
            IPlantCache plantCache,
            IOptionsSnapshot<AuthenticatorOptions> authenticatorOptions,
            ISettingRepository settingRepository)
        {
            _logger = logger;
            _mediator = mediator;
            _claimsPrincipalProvider = claimsPrincipalProvider;
            _currentUserSetter = currentUserSetter;
            _claimsTransformation = claimsTransformation;
            _plantSetter = plantSetter;
            _authenticator = authenticator;
            _plantCache = plantCache;
            _preservationApiOid = authenticatorOptions.Value.PreservationApiObjectId;
            _settingRepository = settingRepository;
            _machine = Environment.MachineName;
        }

        public async Task Synchronize(CancellationToken cancellationToken)
        {
            var runOnMachine = _settingRepository.GetByCodeAsync("OnMachine").Result;
            if (runOnMachine == null || runOnMachine.Value != _machine)
            {
                _logger.LogInformation($"SynchronizationService: Not enabled on {_machine}. Exiting ...");
                return;
            }

            _authenticator.AuthenticationType = AuthenticationType.AsApplication;

            _currentUserSetter.SetCurrentUserOid(_preservationApiOid);

            var currentUser = _claimsPrincipalProvider.GetCurrentClaimsPrincipal();
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimsExtensions.Oid, _preservationApiOid.ToString()));
            currentUser.AddIdentity(claimsIdentity);

            var saveChanges = _settingRepository.GetByCodeAsync("SaveChanges").Result;
            foreach (var plant in await _plantCache.GetPlantIdsWithAccessForUserAsync(_preservationApiOid))
            {
                _logger.LogInformation($"SynchronizationService: Synchronizing plant {plant}...");

                try
                {
                    _plantSetter.SetPlant(plant);
                    await _claimsTransformation.TransformAsync(currentUser);

                    var startTime = TimeService.UtcNow;

                    var result = await _mediator.Send(new SyncTagDataCommand(saveChanges?.Value == "true"));

                    var endTime = TimeService.UtcNow;

                    _logger.LogInformation($"SynchronizationService: Plant {plant} synchronized. Duration: {(endTime - startTime).TotalSeconds}s.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"SynchronizationService: Error synchronizing plant {plant}...");
                }
            }
        }
    }
}
