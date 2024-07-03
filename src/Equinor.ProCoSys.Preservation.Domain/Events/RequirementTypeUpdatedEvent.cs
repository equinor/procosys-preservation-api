using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementTypeUpdatedEvent : IDomainEvent
{
    public RequirementType RequirementType { get; }
    public RequirementTypeUpdatedEvent(RequirementType requirementType) => RequirementType = requirementType;
}
