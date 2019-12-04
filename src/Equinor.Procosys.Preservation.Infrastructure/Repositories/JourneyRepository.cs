using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class JourneyRepository : RepositoryBase<Journey>, IJourneyRepository
    {
        public JourneyRepository(PreservationContext context)
            : base(context.Set<Journey>(), context)
        {
        }
    }
}
