using Equinor.ProCoSys.Preservation.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public class CrossPlantAccessChecker : ICrossPlantAccessChecker
    {
        private readonly ILogger<CrossPlantAccessChecker> _logger;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IOptionsMonitor<AuthorizationOptions> _attachmentOptions;

        public CrossPlantAccessChecker(ICurrentUserProvider currentUserProvider,
            IOptionsMonitor<AuthorizationOptions> attachmentOptions,
            ILogger<CrossPlantAccessChecker> logger)
        {
            _currentUserProvider = currentUserProvider;
            _attachmentOptions = attachmentOptions;
            _logger = logger;
        }

        public bool HasCurrentUserAccessToCrossPlant()
        {
            _logger.LogInformation($"Allowed CrossPlantUserOids: '{_attachmentOptions.CurrentValue}'");
            return _attachmentOptions.CurrentValue.CrossPlantUserOids()
                .Contains(_currentUserProvider.GetCurrentUserOid());
        }
    }
}
