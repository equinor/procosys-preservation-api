using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementUpdatedFieldEvent : IPlantEntityEvent<Field>, IDomainEvent
{
    public RequirementUpdatedFieldEvent(Field field) => Entity = field;
    public Field Entity { get; }
}
