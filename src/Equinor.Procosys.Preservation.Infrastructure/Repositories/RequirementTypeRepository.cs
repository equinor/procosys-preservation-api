using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class RequirementTypeRepository : RepositoryBase<RequirementType>, IRequirementTypeRepository
    {
        public RequirementTypeRepository(PreservationContext context)
            : base(context.RequirementTypes)
        {
        }
    }
}
