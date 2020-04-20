using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Services;
using Microsoft.AspNetCore.Authentication;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        public static string ProjectPrefix = "PCS_PROJECT##";
        public static string ContentRestrictionPrefix = "PCS_CONTENTRESTRICTION##";
        public static string NoRestrictions = "%";

        private readonly IPermissionService _permissionService;

        public ClaimsTransformation(IPermissionService permissionService) => _permissionService = permissionService;

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Claims.All(c => c.Type != ClaimTypes.Role))
            {
                await AddRoleForAllPermissionsAsync(principal);
                await AddUserDataClaimForAllProjects(principal);
                await AddUserDataClaimForAllContentRestrictionsAsync(principal);
            }

            return principal;
        }

        private async Task AddUserDataClaimForAllProjects(ClaimsPrincipal principal)
        {
            var oid = principal.Claims.TryGetOid();
            if (oid.HasValue)
            {
                var projects = await _permissionService.GetProjectsForUserOidAsync(oid.Value);
                var claimsIdentity = new ClaimsIdentity();
                projects.ToList().ForEach(project => claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, $"{ProjectPrefix}{project}")));
                principal.AddIdentity(claimsIdentity);
            }
        }

        private async Task AddRoleForAllPermissionsAsync(ClaimsPrincipal principal)
        {
            var oid = principal.Claims.TryGetOid();
            if (oid.HasValue)
            {
                var permissions = await _permissionService.GetPermissionsForUserOidAsync(oid.Value);
                var claimsIdentity = new ClaimsIdentity();
                permissions.ToList().ForEach(perm => claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, perm)));
                principal.AddIdentity(claimsIdentity);
            }
        }

        private async Task AddUserDataClaimForAllContentRestrictionsAsync(ClaimsPrincipal principal)
        {
            var oid = principal.Claims.TryGetOid();
            if (oid.HasValue)
            {
                var contentRestrictions = await _permissionService.GetContentRestrictionsForUserOidAsync(oid.Value);
                var claimsIdentity = new ClaimsIdentity();
                contentRestrictions.ToList().ForEach(contentRestriction => claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, $"{ContentRestrictionPrefix}{contentRestriction}")));
                principal.AddIdentity(claimsIdentity);
            }
        }
    }
}
