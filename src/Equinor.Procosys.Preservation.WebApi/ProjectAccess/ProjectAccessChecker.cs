using System;
using System.Linq;
using System.Security.Claims;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public class ProjectAccessChecker : IProjectAccessChecker
    {
        private readonly IHttpContextAccessor _accessor;

        public ProjectAccessChecker(IHttpContextAccessor accessor) => _accessor = accessor;

        public bool HasCurrentUserAccessToProject(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(nameof(projectName));
            }

            var user = _accessor.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                var userDataClaimWithProject = $"{ClaimsTransformation.ProjectPrefix}{projectName}";
                return user.Claims.Any(c => c.Type == ClaimTypes.UserData && c.Value == userDataClaimWithProject);
            }

            return true;
        }
    }
}
