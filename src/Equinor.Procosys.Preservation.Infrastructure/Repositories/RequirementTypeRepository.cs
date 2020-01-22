using System.Collections.Generic;
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
                //.Where(rt => rt.RequirementDefinitions.Any(rd => rd.Id == requirementDefinitionId)) Henning > to be discussed
                .SelectMany(rt => rt.RequirementDefinitions)
                .Where(rd => rd.Id == requirementDefinitionId)
                .FirstOrDefaultAsync();

        public Task<List<RequirementDefinition>> GetRequirementDefinitionsByIdsAsync(IList<int> requirementDefinitionIds)
            => DefaultQuery
                //.Where(rt => rt.RequirementDefinitions.Any(rd => requirementDefinitionIds.Contains(rd.Id)))
                .SelectMany(rt => rt.RequirementDefinitions)
                .Where(rd => requirementDefinitionIds.Contains(rd.Id))
                .ToListAsync();

    }
}
