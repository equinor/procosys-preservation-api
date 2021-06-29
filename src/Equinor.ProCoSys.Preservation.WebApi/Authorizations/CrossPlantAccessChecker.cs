using Equinor.ProCoSys.Preservation.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public class CrossPlantAccessChecker : ICrossPlantAccessChecker
    {
        private readonly ILogger<CrossPlantAccessChecker> _logger;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IOptionsMonitor<AuthorizationOptions> _authorizationOptions;

        public CrossPlantAccessChecker(ICurrentUserProvider currentUserProvider,
            IOptionsMonitor<AuthorizationOptions> authorizationOptions,
            ILogger<CrossPlantAccessChecker> logger)
        {
            _currentUserProvider = currentUserProvider;
            _authorizationOptions = authorizationOptions;
            _logger = logger;
        }

        public bool HasCurrentUserAccessToCrossPlant()
        {
            _logger.LogInformation($"Allowed CrossPlantUserOids: '{_authorizationOptions.CurrentValue}'");
            return _authorizationOptions.CurrentValue.CrossPlantUserOids()
                .Contains(_currentUserProvider.GetCurrentUserOid());
        }
    }
}
