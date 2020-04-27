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

        public ClaimsPrincipal GetCurrentUser() => _accessor.HttpContext.User;

        public Guid GetCurrentUserOid()
        {
            var userOid = TryGetCurrentUserOid();

            if (userOid.HasValue)
            {
                return userOid.Value;
            }
            throw new Exception("Unable to determine current user");
        }

        public Guid? TryGetCurrentUserOid() => GetCurrentUser().Claims.TryGetOid();

        public bool IsCurrentUserAuthenticated() => GetCurrentUser().Identity.IsAuthenticated;
    }
}
