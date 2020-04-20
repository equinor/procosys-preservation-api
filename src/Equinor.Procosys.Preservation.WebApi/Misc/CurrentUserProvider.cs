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
            var oid = _accessor.HttpContext.User.Claims.TryGetOid();
            if (oid.HasValue)
            {
                return oid.Value;
            }
            throw new Exception("Unable to determine current user");
        }
    }
}
