using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementDefinitionDeletedEvent : IDomainEvent
{
    public RequirementDefinition RequirementDefinition { get; }
    public RequirementDefinitionDeletedEvent(RequirementDefinition requirementDefinition) => RequirementDefinition = requirementDefinition;
}
