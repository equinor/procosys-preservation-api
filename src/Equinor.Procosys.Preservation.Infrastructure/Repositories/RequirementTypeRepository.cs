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
                .SelectMany(rt => rt.RequirementDefinitions)
                .FirstOrDefaultAsync(rd => rd.Id == requirementDefinitionId);

        public Task<List<RequirementDefinition>> GetRequirementDefinitionsByIdsAsync(IList<int> requirementDefinitionIds)
            => DefaultQuery
                .SelectMany(rt => rt.RequirementDefinitions)
                .Where(rd => requirementDefinitionIds.Contains(rd.Id))
                .ToListAsync();

        public Task<Field> GetFieldByIdAsync(int fieldId)
            => DefaultQuery
                .SelectMany(rt => rt.RequirementDefinitions)
                .SelectMany(rd => rd.Fields)
                .FirstOrDefaultAsync(f => f.Id == fieldId);

        public Task<RequirementDefinition> GetRequirementDefinitionByFieldIdAsync(int fieldId)
            => DefaultQuery
                .SelectMany(rt => rt.RequirementDefinitions)
                .Where(rt => rt.Fields.Any(f => f.Id == fieldId))
                .FirstOrDefaultAsync();
    }
}
