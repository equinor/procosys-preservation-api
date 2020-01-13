using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class RequirementTypeRepository : RepositoryBase<RequirementType>, IRequirementTypeRepository
    {
        public RequirementTypeRepository(PreservationContext context)
            : base(context.RequirementTypes, context.RequirementTypes.Include(x => x.RequirementDefinitions).ThenInclude(x => x.Fields))
        {
        }

        public Task<RequirementDefinition> GetRequirementDefinitionByIdAsync(int requirementDefinitionId)
            => DefaultQuery
                .Where(rt => rt.RequirementDefinitions.Any(rd => rd.Id == requirementDefinitionId))
                .SelectMany(rt => rt.RequirementDefinitions)
                .Where(rd => rd.Id == requirementDefinitionId)
                .FirstOrDefaultAsync();
    }
}
