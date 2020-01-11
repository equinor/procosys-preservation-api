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

        public RequirementDefinition GetRequirementDefinitionById(int requirementDefinitionId)
        {
            var requirementType = DefaultQuery
                .Where(rt => rt.RequirementDefinitions.Any(rd => rd.Id == requirementDefinitionId))
                .FirstOrDefaultAsync().Result;
                
            return requirementType
                ?.RequirementDefinitions
                .FirstOrDefault(rd => rd.Id == requirementDefinitionId);
        }
    }
}
