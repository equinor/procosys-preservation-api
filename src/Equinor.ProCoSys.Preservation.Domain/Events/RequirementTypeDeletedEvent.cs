using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementTypeDeletedEvent : IPlantEntityEvent<RequirementType>, IDomainEvent
{
    public RequirementType Entity { get; }
    public RequirementTypeDeletedEvent(RequirementType requirementType) => Entity = requirementType;
}
