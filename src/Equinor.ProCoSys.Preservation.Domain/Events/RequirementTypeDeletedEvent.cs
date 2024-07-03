using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementTypeDeletedEvent : IDomainEvent
{
    public RequirementType RequirementType { get; }
    public RequirementTypeDeletedEvent(RequirementType requirementType) => RequirementType = requirementType;
}
