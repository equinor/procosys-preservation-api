using System;
using System.Security.Claims;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserProvider(IHttpContextAccessor accessor) => _accessor = accessor;

        public ClaimsPrincipal CurrentUser() => _accessor.HttpContext.User;

        public Guid GetCurrentUser() // will be renamed to GetCurrentUserOid
        {
            var userOid = TryGetCurrentUserOid();

            if (userOid.HasValue)
            {
                return userOid.Value;
            }
            throw new Exception("Unable to determine current user");
        }

        public Guid? TryGetCurrentUserOid() => CurrentUser().Claims.TryGetOid();

        public bool IsCurrentUserAuthenticated() => CurrentUser().Identity.IsAuthenticated;
    }
}
