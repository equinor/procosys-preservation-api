using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class ResponsibleRepository : RepositoryBase<Responsible>, IResponsibleRepository
    {
        public ResponsibleRepository(PreservationContext context) : base(context.Responsibles, context)
        {
        }
    }
}
