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
    }
}
