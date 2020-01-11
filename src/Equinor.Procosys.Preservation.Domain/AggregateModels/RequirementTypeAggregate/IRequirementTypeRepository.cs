using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate
{
    public interface IRequirementTypeRepository : IRepository<RequirementType>
    {
        RequirementDefinition GetRequirementDefinitionById (int requirementDefinitionId);
    }
}
