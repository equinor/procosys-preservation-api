using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Equinor.ProCoSys.Auth.Authorization;
using Equinor.ProCoSys.Auth.Misc;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public class RestrictionRolesChecker : IRestrictionRolesChecker
    {
        private readonly IClaimsPrincipalProvider _claimsPrincipalProvider;

        public RestrictionRolesChecker(IClaimsPrincipalProvider claimsPrincipalProvider) => _claimsPrincipalProvider = claimsPrincipalProvider;

        public bool HasCurrentUserExplicitNoRestrictions()
        {
            var claimWithRestrictionRole = GetRestrictionRoleClaims(_claimsPrincipalProvider.GetCurrentClaimsPrincipal().Claims);

            // the rule for saying that a user do not have any restriction, is that user has one and only one restriction role with value %
            return claimWithRestrictionRole.Count == 1 && HasRestrictionRoleClaim(claimWithRestrictionRole, ClaimsTransformation.NoRestrictions);
        }

        public bool HasCurrentUserExplicitAccessToContent(string responsibleCode)
        {
            if (string.IsNullOrEmpty(responsibleCode))
            {
                return false;
            }
            
            var claimWithRestrictionRole = GetRestrictionRoleClaims(_claimsPrincipalProvider.GetCurrentClaimsPrincipal().Claims);
            return HasRestrictionRoleClaim(claimWithRestrictionRole, responsibleCode);
        }

        private bool HasRestrictionRoleClaim(IEnumerable<Claim> claims, string responsibleCode)
        {
            var contentRestrictionClaimValue = ClaimsTransformation.GetRestrictionRoleClaimValue(responsibleCode);
            return claims.Any(c => c.Type == ClaimTypes.UserData && c.Value == contentRestrictionClaimValue);
        }

        private List<Claim> GetRestrictionRoleClaims(IEnumerable<Claim> claims)
            => claims.Where(c =>
                    c.Type == ClaimTypes.UserData &&
                    c.Value.StartsWith(ClaimsTransformation.RestrictionRolePrefix))
                .ToList();
    }
}
