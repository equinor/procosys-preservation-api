using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public static class ClaimsExtensions
    {
        public const string Oid = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public const string SurName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        public const string FullName = "name";

        public static Guid? TryGetOid(this IEnumerable<Claim> claims)
        {
            var oidClaim = claims.SingleOrDefault(c => c.Type == Oid);
            if (Guid.TryParse(oidClaim?.Value, out var oid))
            {
                return oid;
            }

            return null;
        }

        public static string TryGetGivenName(this IEnumerable<Claim> claims)
        {
            var givenName = claims.SingleOrDefault(c => c.Type == GivenName);

            return givenName?.Value;
        }

        public static string TryGetSurName(this IEnumerable<Claim> claims)
        {
            var surName = claims.SingleOrDefault(c => c.Type == SurName);

            return surName?.Value;
        }

        public static string TryGetFullName(this IEnumerable<Claim> claims)
        {
            var fullName = claims.SingleOrDefault(c => c.Type == FullName);

            return fullName?.Value;
        }
    }
}
