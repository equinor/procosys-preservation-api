using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Repositories
{
    public class HistoryRepository : RepositoryBase<History>, IHistoryRepository
    {
        public HistoryRepository(PreservationContext context)
            : base(context, context.History)
        {
        }
    }
}
