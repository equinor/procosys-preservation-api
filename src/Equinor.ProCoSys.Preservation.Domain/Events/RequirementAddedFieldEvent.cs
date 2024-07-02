using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementAddedFieldEvent : IDomainEvent
{
    public RequirementAddedFieldEvent(RequirementDefinition requirementDefinition, Field field)
    {
        this.RequirementDefinition = requirementDefinition;
        this.Field = field;
    }

    public RequirementDefinition RequirementDefinition { get; }
    public Field Field { get; }
}
