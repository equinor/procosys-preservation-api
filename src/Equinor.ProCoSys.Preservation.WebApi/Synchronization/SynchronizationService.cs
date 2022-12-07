﻿using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.FillPCSGuids;
using Equinor.ProCoSys.Preservation.Domain;
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
        private readonly IClaimsProvider _claimsProvider;
        private readonly IPlantSetter _plantSetter;
        private readonly ICurrentUserSetter _currentUserSetter;
        private readonly IClaimsTransformation _claimsTransformation;
        private readonly IAuthenticator _authenticator;
        private readonly IPlantCache _plantCache;
        private readonly IOptionsSnapshot<SynchronizationOptions> _options;

        public SynchronizationService(
            ILogger<SynchronizationService> logger,
            IMediator mediator,
            IClaimsProvider claimsProvider,
            IPlantSetter plantSetter,
            ICurrentUserSetter currentUserSetter,
            IClaimsTransformation claimsTransformation,
            IAuthenticator authenticator,
            IPlantCache plantCache,
            IOptionsSnapshot<SynchronizationOptions> options,
            IOptionsSnapshot<AuthenticatorOptions> authenticatorOptions)
        {
            _logger = logger;
            _mediator = mediator;
            _claimsProvider = claimsProvider;
            _currentUserSetter = currentUserSetter;
            _claimsTransformation = claimsTransformation;
            _plantSetter = plantSetter;
            _authenticator = authenticator;
            _plantCache = plantCache;
            _options = options;
            _preservationApiOid = authenticatorOptions.Value.PreservationApiObjectId;
        }

        public async Task Synchronize(CancellationToken cancellationToken)
        {
            _authenticator.AuthenticationType = AuthenticationType.AsApplication;

            _currentUserSetter.SetCurrentUserOid(_preservationApiOid);

            var currentUser = _claimsProvider.GetCurrentUser();
            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimsExtensions.Oid, _preservationApiOid.ToString()));
            currentUser.AddIdentity(claimsIdentity);

            foreach (var plant in await _plantCache.GetPlantIdsWithAccessForUserAsync(_preservationApiOid))
            {
                _logger.LogInformation($"FillPCSGuids: Synchronizing plant {plant}...");

                try
                {
                    _plantSetter.SetPlant(plant);
                    await _claimsTransformation.TransformAsync(currentUser);

                    var startTime = TimeService.UtcNow;

                    var result = await _mediator.Send(new FillPCSGuidsCommand(false));

                    var endTime = TimeService.UtcNow;

                    _logger.LogInformation($"FillPCSGuids: Plant {plant} synchronized. Duration: {(endTime - startTime).TotalSeconds}s.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"FillPCSGuids: Error synchronizing plant {plant}...");
                }
            }
        }
    }
}
