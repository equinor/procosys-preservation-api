using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Repositories
{
    public class RequirementTypeRepository : RepositoryBase<RequirementType>, IRequirementTypeRepository
    {
        public RequirementTypeRepository(PreservationContext context)
            : base(context, context.RequirementTypes,
                context.RequirementTypes.Include(x => x.RequirementDefinitions).ThenInclude(x => x.Fields))
        {

        }

        public Task<RequirementDefinition> GetRequirementDefinitionByIdAsync(int requirementDefinitionId)
            => DefaultQuery
                .SelectMany(rt => rt.RequirementDefinitions)
                .SingleOrDefaultAsync(rd => rd.Id == requirementDefinitionId);

        public Task<List<RequirementDefinition>> GetRequirementDefinitionsByIdsAsync(IList<int> requirementDefinitionIds)
            => DefaultQuery
                .SelectMany(rt => rt.RequirementDefinitions)
                .Where(rd => requirementDefinitionIds.Contains(rd.Id))
                .ToListAsync();

        public Task<RequirementType> GetRequirementTypeByRequirementDefinitionGuidAsync(Guid requirementDefinitionGuid)
            => DefaultQuery
                .Where(rt => rt.RequirementDefinitions.Any(rd => rd.Guid == requirementDefinitionGuid))
                .FirstOrDefaultAsync();

        public void RemoveRequirementDefinition(RequirementDefinition requirementDefinition)
            => _context.RequirementDefinitions.Remove(requirementDefinition);

        public void RemoveField(Field field)
            => _context.Fields.Remove(field);
    }
}
