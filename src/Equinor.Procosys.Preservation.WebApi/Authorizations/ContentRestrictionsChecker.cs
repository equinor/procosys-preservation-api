using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class ContentRestrictionsChecker : IContentRestrictionsChecker
    {
        private readonly ICurrentUserProvider _currentUserProvider;

        public ContentRestrictionsChecker(ICurrentUserProvider currentUserProvider) => _currentUserProvider = currentUserProvider;

        public bool HasCurrentUserAnyRestrictions()
        {
            var userDataClaimWithContentRestriction = _currentUserProvider.CurrentUser.Claims.GetContentRestrictionClaims();
            return userDataClaimWithContentRestriction.Count > 1 &&
                   userDataClaimWithContentRestriction.First().Value != ClaimsTransformation.NoRestrictions;
        }

        public bool HasCurrentUserExplicitAccessToContent(string responsibleCode)
        {
            if (string.IsNullOrEmpty(responsibleCode))
            {
                throw new ArgumentNullException(nameof(responsibleCode));
            }
            
            return _currentUserProvider.CurrentUser.Claims.HasContentRestrictionClaim(responsibleCode);
        }
    }
}
