using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementDefinitionDeletedEvent : IPlantEntityEvent<RequirementDefinition>, IDomainEvent
{
    public RequirementDefinition Entity { get; }
    public RequirementDefinitionDeletedEvent(RequirementDefinition requirementDefinition) => Entity = requirementDefinition;
}
