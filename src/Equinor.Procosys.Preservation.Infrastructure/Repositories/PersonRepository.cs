using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class PersonRepository : RepositoryBase<Person>, IPersonRepository
    {
        public PersonRepository(PreservationContext context)
            : base(context.Persons, context.Persons.Include(p => p.SavedFilters))
        {
        }

        public Task<Person> GetByOidAsync(Guid oid)
            => DefaultQuery.SingleOrDefaultAsync(p => p.Oid == oid);
    }
}
