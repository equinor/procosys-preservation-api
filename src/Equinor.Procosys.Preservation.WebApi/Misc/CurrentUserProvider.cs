using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private const string Oid = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserProvider(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public Guid GetCurrentUser()
        {
            var oidClaim = _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == Oid);
            if (Guid.TryParse(oidClaim?.Value, out var oid))
            {
                return oid;
            }
            throw new Exception("Unable to determine current user");
        }
    }
}
