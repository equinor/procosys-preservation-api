using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.WebApi.Caches;
using Microsoft.AspNetCore.Authentication;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        public static string ProjectPrefix = "PCS_PROJECT##";
        public static string ContentRestrictionPrefix = "PCS_CONTENTRESTRICTION##";
        public static string NoRestrictions = "%";

        private readonly IPlantProvider _plantProvider;
        private readonly IPlantCache _plantCache;
        private readonly IPermissionCache _permissionCache;

        public ClaimsTransformation(IPlantProvider plantProvider, IPlantCache plantCache, IPermissionCache permissionCache)
        {
            _plantProvider = plantProvider;
            _plantCache = plantCache;
            _permissionCache = permissionCache;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var userOid = principal.Claims.TryGetOid();
            if (!userOid.HasValue)
            {
                // not add claims if no oid found (I.e user not authenticated yet)
                return principal;
            }

            var plantId = _plantProvider.Plant;

            if (string.IsNullOrEmpty(plantId))
            {
                // not add plant specific claims if no plant given in request
                return principal;
            }

            if (!await _plantCache.IsValidPlantForUserAsync(plantId, userOid.Value))
            {
                // not add plant specific claims if plant not among plants for user
                return principal;
            }

            if (principal.Claims.All(c => c.Type != ClaimTypes.Role))
            {
                await AddRoleForAllPermissionsAsync(principal, plantId, userOid.Value);
                await AddUserDataClaimForAllProjects(principal, plantId, userOid.Value);
                await AddUserDataClaimForAllContentRestrictionsAsync(principal, plantId, userOid.Value);
            }

            return principal;
        }

        private async Task AddUserDataClaimForAllProjects(ClaimsPrincipal principal, string plantId, Guid userOid)
        {
            var projectNames = await _permissionCache.GetProjectNamesForUserOidAsync(plantId, userOid);
            var claimsIdentity = new ClaimsIdentity();
            projectNames?.ToList().ForEach(
                projectName => claimsIdentity.AddClaim(new Claim(ClaimTypes.UserData, GetProjectClaimValue(projectName))));
            principal.AddIdentity(claimsIdentity);
        }

        private async Task AddRoleForAllPermissionsAsync(ClaimsPrincipal principal, string plantId, Guid userOid)
        {
            var permissions = await _permissionCache.GetPermissionsForUserAsync(plantId, userOid);
            var claimsIdentity = new ClaimsIdentity();
            permissions?.ToList().ForEach(
                permission => claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, permission)));
            principal.AddIdentity(claimsIdentity);
        }

        private async Task AddUserDataClaimForAllContentRestrictionsAsync(ClaimsPrincipal principal, string plantId, Guid userOid)
        {
            var contentRestrictions = await _permissionCache.GetContentRestrictionsForUserOidAsync(plantId, userOid);
            var claimsIdentity = new ClaimsIdentity();
            contentRestrictions?.ToList().ForEach(
                contentRestriction => claimsIdentity.AddClaim(
                    new Claim(ClaimTypes.UserData, GetContentRestrictionClaimValue(contentRestriction))));
            
            principal.AddIdentity(claimsIdentity);
        }

        public static string GetProjectClaimValue(string projectName) => $"{ProjectPrefix}{projectName}";

        public static string GetContentRestrictionClaimValue(string contentRestriction) => $"{ContentRestrictionPrefix}{contentRestriction}";
    }
}
