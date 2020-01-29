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
        private readonly IPersonRepository _personRepository;

        public CurrentUserProvider(IHttpContextAccessor accessor, IPersonRepository personRepository)
        {
            _accessor = accessor;
            _personRepository = personRepository;
        }

        public async Task<Person> GetCurrentUserAsync()
        {
            var oidString = _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == Oid).Value;
            var oid = Guid.Parse(oidString);
            var user = await _personRepository.GetByOidAsync(oid);
            return user;
        }
    }
}
