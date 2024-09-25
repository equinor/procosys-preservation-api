using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementDeletedFieldEvent : IPlantEntityEvent<Field>, IDomainEvent
{
    public RequirementDeletedFieldEvent(RequirementDefinition requirementDefinition, Field field)
    {
        this.RequirementDefinition = requirementDefinition;
        Entity = field;
    }

    public RequirementDefinition RequirementDefinition { get; }
    public Field Entity { get; }
}
