using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class ContentRestrictionsChecker : IContentRestrictionsChecker
    {
        private readonly IHttpContextAccessor _accessor;

        public ContentRestrictionsChecker(IHttpContextAccessor accessor) => _accessor = accessor;

        public bool HasCurrentUserAnyRestrictions()
        {
            var authenticatedUser = GetAuthenticatedUser();
            if (authenticatedUser != null)
            {
                var userDataClaimWithContentRestriction = authenticatedUser.Claims.GetContentRestrictionClaims();
                return userDataClaimWithContentRestriction.Count > 1 && userDataClaimWithContentRestriction.First().Value !=
                       ClaimsTransformation.NoRestrictions;
            }

            return false;
        }

        public bool HasCurrentUserExplicitAccessToContent(string responsibleCode)
        {
            var authenticatedUser = GetAuthenticatedUser();
            
            return authenticatedUser != null && authenticatedUser.Claims.HasContentRestrictionClaim(responsibleCode);
        }

        private ClaimsPrincipal GetAuthenticatedUser()
        {
            var user = _accessor.HttpContext.User;
            return user.Identity.IsAuthenticated ? user : null;
        }
    }
}
