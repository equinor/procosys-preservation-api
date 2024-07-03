using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementTypeAddedEvent : IDomainEvent
{
    public RequirementType RequirementType { get; }
    public RequirementTypeAddedEvent(RequirementType requirementType) => RequirementType = requirementType;
}
