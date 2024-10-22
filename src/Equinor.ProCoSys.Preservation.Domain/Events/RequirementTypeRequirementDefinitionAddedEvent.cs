using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementTypeRequirementDefinitionAddedEvent : IPlantEntityEvent<RequirementType>, IDomainEvent
{
    public RequirementTypeRequirementDefinitionAddedEvent(RequirementType entity, RequirementDefinition requirementDefinition)
    {
        Entity = entity;
        RequirementDefinition = requirementDefinition;
    }

    public RequirementType Entity { get; }
    public RequirementDefinition RequirementDefinition { get; }
}
