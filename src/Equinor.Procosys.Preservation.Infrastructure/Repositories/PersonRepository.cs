using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class PersonRepository : RepositoryBase<Person>, IPersonRepository
    {
        public PersonRepository(PreservationContext context)
            : base(context.Persons)
        {
        }
    }
}
