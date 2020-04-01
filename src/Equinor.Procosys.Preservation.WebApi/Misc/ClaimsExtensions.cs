using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public static class ClaimsExtensions
    {
        private const string Oid = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        public static Guid? TryGetOid(this IEnumerable<Claim> claims)
        {
            var oidClaim = claims.FirstOrDefault(c => c.Type == Oid);
            if (Guid.TryParse(oidClaim?.Value, out var oid))
            {
                return oid;
            }

            return null;
        }
    }
}
