using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementDefinitionUpdatedEvent : IPlantEntityEvent<RequirementDefinition>, IDomainEvent
{
    public RequirementDefinition Entity { get; }
    public RequirementDefinitionUpdatedEvent(RequirementDefinition requirementDefinition) => Entity = requirementDefinition;
}
