using System;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserProvider(IHttpContextAccessor accessor) => _accessor = accessor;

        public Guid GetCurrentUser()
        {
            var guid = TryGetCurrentUserOid();

            if (guid.HasValue)
            {
                return guid.Value;
            }
            throw new Exception("Unable to determine current user");
        }

        public Guid? TryGetCurrentUserOid()
            => _accessor.HttpContext.User.Claims.TryGetOid();
    }
}
