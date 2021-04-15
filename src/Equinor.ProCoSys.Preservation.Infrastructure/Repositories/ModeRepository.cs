using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Repositories
{
    public class ModeRepository : RepositoryBase<Mode>, IModeRepository
    {
        public ModeRepository(PreservationContext context)
            : base(context, context.Modes)
        {
        }
    }
}
