using System.Linq;
using Equinor.ProCoSys.Preservation.Domain;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public class CrossPlantAccessChecker : ICrossPlantAccessChecker
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IOptionsMonitor<AuthorizationOptions> _attachmentOptions;

        public CrossPlantAccessChecker(ICurrentUserProvider currentUserProvider,
            IOptionsMonitor<AuthorizationOptions> attachmentOptions)
        {
            _currentUserProvider = currentUserProvider;
            _attachmentOptions = attachmentOptions;
        }

        public bool HasCurrentUserAccessToCrossPlant()
            => _attachmentOptions.CurrentValue.CrossPlantUserOids.Contains(_currentUserProvider.GetCurrentUserOid());
    }
}
