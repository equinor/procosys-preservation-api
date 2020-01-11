using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public interface IRequirementTypeRepository : IRepository<RequirementType>
    {
        Task<RequirementDefinition> GetRequirementDefinitionByIdAsync(int requirementDefinitionId);
    }
}
