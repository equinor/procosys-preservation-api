using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Services;
using Microsoft.AspNetCore.Authentication;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IPermissionService _permissionService;

        public ClaimsTransformation(IPermissionService permissionService) => _permissionService = permissionService;

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Claims.All(c => c.Type != ClaimTypes.Role))
            {
                await AddRoleForAllPermissionsAsync(principal);
            }

            return principal;
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
    }
}
