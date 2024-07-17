using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Repositories
{
    public class PersonRepository : RepositoryBase<Person>, IPersonRepository
    {
        public PersonRepository(PreservationContext context)
            : base(context, context.Persons) { }

        public Task<Person> GetByOidAsync(Guid oid)
            => DefaultQuery.SingleOrDefaultAsync(p => p.Guid == oid);

        public Task<Person> GetWithSavedFiltersByOidAsync(Guid oid)
            => DefaultQuery
                .Include(p => p.SavedFilters)
                .SingleOrDefaultAsync(p => p.Guid == oid);

        public Task<Person> GetReadOnlyByIdAsync(int personId)
            => DefaultQuery.AsNoTracking().SingleOrDefaultAsync(p => p.Id == personId);

        public void RemoveSavedFilter(SavedFilter savedFilter)
            => _context.SavedFilters.Remove(savedFilter);
    }
}
