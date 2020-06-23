using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncProjects;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncResponsibles;
using Equinor.Procosys.Preservation.Command.SyncCommands.SyncTagFunctions;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Equinor.Procosys.Preservation.WebApi.Misc;
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
        private readonly IMediator _mediator;
        private readonly IClaimsProvider _claimsProvider;
        private readonly IPlantSetter _plantSetter;
        private readonly ICurrentUserSetter _currentUserSetter;
        private readonly IBearerTokenSetter _bearerTokenSetter;
        private readonly IClaimsTransformation _claimsTransformation;
        private readonly IAuthenticator _authenticator;
        private readonly IPlantCache _plantCache;

        public SynchronizationService(
            ILogger<SynchronizationService> logger,
            IMediator mediator,
            IClaimsProvider claimsProvider,
            IPlantSetter plantSetter,
            ICurrentUserSetter currentUserSetter,
            IBearerTokenSetter bearerTokenSetter,
            IClaimsTransformation claimsTransformation,
            IAuthenticator authenticator,
            IPlantCache plantCache,
            IOptionsMonitor<SynchronizationOptions> options)
        {
            _logger = logger;
            _mediator = mediator;
            _claimsProvider = claimsProvider;
            _currentUserSetter = currentUserSetter;
            _claimsTransformation = claimsTransformation;
            _plantSetter = plantSetter;
            _authenticator = authenticator;
            _bearerTokenSetter = bearerTokenSetter;
            _plantCache = plantCache;

            _synchronizationUserOid = options.CurrentValue.UserOid;
        }

        public async Task Synchronize(CancellationToken cancellationToken)
        {
            var bearerToken = await _authenticator.GetBearerTokenAsync();
            _bearerTokenSetter.SetBearerToken(bearerToken);

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
                await SynchronizeProjects();
                await SynchronizeResponsibles();
                await SynchronizeTagFunctions();
                var endTime = TimeService.UtcNow;

                _logger.LogInformation($"Plant {plant} synchronized. Duration: {(endTime - startTime).TotalSeconds}s.");
            }
        }

        private async Task SynchronizeProjects()
        {
            _logger.LogInformation($"Synchronizing projects");

            var result = await _mediator.Send(new SyncProjectsCommand());

            if (result.ResultType == ServiceResult.ResultType.Ok)
            {
                _logger.LogWarning($"Synchronizing projects complete.");
            }
            else
            {
                _logger.LogWarning($"Synchronizing projects failed.");
            }
        }

        private async Task SynchronizeResponsibles()
        {
            _logger.LogInformation($"Synchronizing responsibles");

            var result = await _mediator.Send(new SyncResponsiblesCommand());

            if (result.ResultType == ServiceResult.ResultType.Ok)
            {
                _logger.LogWarning($"Synchronizing responsibles complete.");
            }
            else
            {
                _logger.LogWarning($"Synchronizing responsibles failed.");
            }
        }

        private async Task SynchronizeTagFunctions()
        {
            _logger.LogInformation($"Synchronizing tag functions");

            var result = await _mediator.Send(new SyncTagFunctionsCommand());

            if (result.ResultType == ServiceResult.ResultType.Ok)
            {
                _logger.LogWarning($"Synchronizing tag functions complete.");
            }
            else
            {
                _logger.LogWarning($"Synchronizing tag functions failed.");
            }
        }
    }
}
