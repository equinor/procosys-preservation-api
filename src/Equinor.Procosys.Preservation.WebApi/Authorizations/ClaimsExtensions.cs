using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public static class ClaimsExtensions
    {
        public const string OidType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        public static Guid? TryGetOid(this IEnumerable<Claim> claims)
        {
            var oidClaim = claims.SingleOrDefault(c => c.Type == OidType);
            if (Guid.TryParse(oidClaim?.Value, out var oid))
            {
                return oid;
            }

            return null;
        }

        public static bool HasProjectClaim(this IEnumerable<Claim> claims, string projectName)
        {
            var userDataClaimWithProject = ClaimsTransformation.GetProjectClaimValue(projectName);
            return claims.Any(c => c.Type == ClaimTypes.UserData && c.Value == userDataClaimWithProject);
        }

        public static bool HasContentRestrictionClaim(this IEnumerable<Claim> claims, string responsibleCode)
        {
            var userDataClaimWithContentRestriction = ClaimsTransformation.GetContentRestrictionClaimValue(responsibleCode);
            return claims.Any(c => c.Type == ClaimTypes.UserData && c.Value == userDataClaimWithContentRestriction);
        }

        public static List<Claim> GetContentRestrictionClaims(this IEnumerable<Claim> claims)
            => claims.Where(c =>
                    c.Type == ClaimTypes.UserData &&
                    c.Value.StartsWith(ClaimsTransformation.ContentRestrictionPrefix))
                .ToList();
    }
}
