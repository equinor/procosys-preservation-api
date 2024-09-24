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

        public bool HasCurrentUserExplicitNoRestrictions() => true;

        public bool HasCurrentUserExplicitAccessToContent(string responsibleCode) => true;

        private bool HasRestrictionRoleClaim(IEnumerable<Claim> claims, string responsibleCode) => true;

        private List<Claim> GetRestrictionRoleClaims(IEnumerable<Claim> claims)
            => claims.Where(c =>
                    c.Type == ClaimTypes.UserData &&
                    c.Value.StartsWith(ClaimsTransformation.RestrictionRolePrefix))
                .ToList();
    }
}
