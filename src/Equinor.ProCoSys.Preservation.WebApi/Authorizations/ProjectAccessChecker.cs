using System.Linq;
using System.Security.Claims;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Auth.Misc;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public class ProjectAccessChecker : IProjectAccessChecker
    {
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;

        public ProjectAccessChecker(IClaimsPrincipalProvider claimsPrincipalProvider) => _claimsPrincipalProvider = claimsPrincipalProvider;

        public bool HasCurrentUserAccessToProject(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                return false;
            }
            
            var userDataClaimWithProject = ClaimsTransformation.GetProjectClaimValue(projectName);
            return _claimsPrincipalProvider.GetCurrentClaimsPrincipal().Claims.Any(c => c.Type == ClaimTypes.UserData && c.Value == userDataClaimWithProject);
        }
    }
}
