using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementTypeUpdatedEvent : IPlantEntityEvent<RequirementType>, IDomainEvent
{
    public RequirementType Entity { get; }
    public RequirementTypeUpdatedEvent(RequirementType requirementType) => Entity = requirementType;
}
