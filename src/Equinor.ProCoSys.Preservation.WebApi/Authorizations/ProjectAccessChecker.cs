using System.Linq;
using System.Security.Claims;
using Equinor.ProCoSys.Preservation.WebApi.Misc;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public class ProjectAccessChecker : IProjectAccessChecker
    {
        private readonly IClaimsProvider _claimsProvider;

        public ProjectAccessChecker(IClaimsProvider claimsProvider) => _claimsProvider = claimsProvider;

        public bool HasCurrentUserAccessToProject(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                return false;
            }
            
            var userDataClaimWithProject = ClaimsTransformation.GetProjectClaimValue(projectName);
            return _claimsProvider.GetCurrentUser().Claims.Any(c => c.Type == ClaimTypes.UserData && c.Value == userDataClaimWithProject);
        }
    }
}
