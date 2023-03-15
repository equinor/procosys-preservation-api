using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public interface IRequirementTypeRepository : IRepository<RequirementType>
    {
        Task<RequirementDefinition> GetRequirementDefinitionByIdAsync (int requirementDefinitionId);
        
        Task<List<RequirementDefinition>> GetRequirementDefinitionsByIdsAsync(IList<int> requirementDefinitionIds);

        void RemoveRequirementDefinition(RequirementDefinition requirementDefinition);
        
        void RemoveField(Field field);
    }
}
