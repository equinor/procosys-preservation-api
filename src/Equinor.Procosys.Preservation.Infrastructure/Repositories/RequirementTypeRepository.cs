using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class RequirementTypeRepository : RepositoryBase<RequirementType>, IRequirementTypeRepository
    {
        readonly PreservationContext _context;
        public RequirementTypeRepository(PreservationContext context)
            : base(context.RequirementTypes, context.RequirementTypes.Include(x => x.RequirementDefinitions).ThenInclude(x => x.Fields))
        {
            _context = context;
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

        public void RemoveRequirementDefinition(RequirementDefinition requirementDefinition) 
            => _context.RequirementDefinitions.Remove(requirementDefinition);
    }
}
