using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementDefinitionAddedEvent : IPlantEntityEvent<RequirementDefinition>, IPostSaveDomainEvent
{
    public RequirementDefinition Entity { get; }
    public RequirementDefinitionAddedEvent(RequirementDefinition requirementDefinition) => Entity = requirementDefinition;
}
