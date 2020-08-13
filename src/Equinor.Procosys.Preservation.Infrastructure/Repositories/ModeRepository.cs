using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class ModeRepository : RepositoryBase<Mode>, IModeRepository
    {
        public ModeRepository(PreservationContext context)
            : base(context.Modes, context)
        {
        }
    }
}
