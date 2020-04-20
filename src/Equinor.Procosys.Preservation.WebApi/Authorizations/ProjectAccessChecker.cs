using System;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
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
            
            return !user.Identity.IsAuthenticated || user.Claims.HasProjectClaim(projectName);
        }
    }
}
