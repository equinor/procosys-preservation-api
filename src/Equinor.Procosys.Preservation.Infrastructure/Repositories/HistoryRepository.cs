using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class HistoryRepository : RepositoryBase<History>, IHistoryRepository
    {
        public HistoryRepository(PreservationContext context)
            : base(context.History)
        {
        }
    }
}
