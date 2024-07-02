using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementDefinitionUpdatedEvent : IDomainEvent
{
    public RequirementDefinition RequirementDefinition { get; }
    public RequirementDefinitionUpdatedEvent(RequirementDefinition requirementDefinition) => RequirementDefinition = requirementDefinition;
}
