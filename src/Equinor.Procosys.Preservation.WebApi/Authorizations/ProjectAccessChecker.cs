using System;
using System.Linq;
using System.Security.Claims;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class ProjectAccessChecker : IProjectAccessChecker
    {
        private readonly ICurrentUserProvider _currentUserProvider;

        public ProjectAccessChecker(ICurrentUserProvider currentUserProvider) => _currentUserProvider = currentUserProvider;

        public bool HasCurrentUserAccessToProject(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(nameof(projectName));
            }
            
            var userDataClaimWithProject = ClaimsTransformation.GetProjectClaimValue(projectName);
            return _currentUserProvider.CurrentUser().Claims.Any(c => c.Type == ClaimTypes.UserData && c.Value == userDataClaimWithProject);
        }
    }
}
