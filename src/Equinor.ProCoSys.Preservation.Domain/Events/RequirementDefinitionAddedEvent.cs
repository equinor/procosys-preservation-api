using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementDefinitionAddedEvent : IPostSaveDomainEvent
{
    public RequirementDefinition RequirementDefinition { get; }
    public RequirementDefinitionAddedEvent(RequirementDefinition requirementDefinition) => RequirementDefinition = requirementDefinition;
}
