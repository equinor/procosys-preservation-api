using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class RequirementTypeAddedEvent : IPlantEntityEvent<RequirementType>, IPostSaveDomainEvent
{
    public RequirementType Entity { get; }
    public RequirementTypeAddedEvent(RequirementType requirementType) => Entity = requirementType;
}
