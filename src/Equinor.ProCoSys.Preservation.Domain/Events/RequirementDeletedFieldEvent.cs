using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementDeletedFieldEvent : IDomainEvent
{
    public RequirementDeletedFieldEvent(RequirementDefinition requirementDefinition, Field field)
    {
        this.RequirementDefinition = requirementDefinition;
        this.Field = field;
    }

    public RequirementDefinition RequirementDefinition { get; }
    public Field Field { get; }
}
