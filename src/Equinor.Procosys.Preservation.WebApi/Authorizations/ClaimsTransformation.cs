using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Services;
using Microsoft.AspNetCore.Authentication;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        public static string PlantPrefix = "PCS_PLANT##";
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
                await AddUserDataClaimForAllPlants(principal);
                await AddUserDataClaimForAllProjects(principal);
                await AddUserDataClaimForAllContentRestrictionsAsync(principal);
            }

            return principal;
        }

        private async Task AddUserDataClaimForAllPlants(ClaimsPrincipal principal)
        {
            var oid = principal.Claims.TryGetOid();
            if (oid.HasValue)
            {
                var plantIds = await _permissionService.GetPlantIdsForUserOidAsync(oid.Value);
                var claimsIdentity = new ClaimsIdentity();
                plantIds?.ToList().ForEach(
                    plantId => claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, GetPlantClaimValue(plantId))));
                principal.AddIdentity(claimsIdentity);
            }
        }

        private async Task AddUserDataClaimForAllProjects(ClaimsPrincipal principal)
        {
            var oid = principal.Claims.TryGetOid();
            if (oid.HasValue)
            {
                var projectNames = await _permissionService.GetProjectNamesForUserOidAsync(oid.Value);
                var claimsIdentity = new ClaimsIdentity();
                projectNames?.ToList().ForEach(
                    projectName => claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, GetProjectClaimValue(projectName))));
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
                permissions?.ToList().ForEach(
                    permission => claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, permission)));
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
                contentRestrictions?.ToList().ForEach(
                    contentRestriction => claimsIdentity.AddClaim(
                        new Claim(ClaimTypes.UserData, GetContentRestrictionClaimValue(contentRestriction))));
                
                principal.AddIdentity(claimsIdentity);
            }
        }

        public static string GetPlantClaimValue(string plantId) => $"{PlantPrefix}{plantId}";
        
        public static string GetProjectClaimValue(string projectName) => $"{ProjectPrefix}{projectName}";

        public static string GetContentRestrictionClaimValue(string contentRestriction) => $"{ContentRestrictionPrefix}{contentRestriction}";
    }
}
