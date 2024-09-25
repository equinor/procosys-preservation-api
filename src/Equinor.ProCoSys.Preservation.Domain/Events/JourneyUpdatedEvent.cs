using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class JourneyUpdatedEvent : IPlantEntityEvent<Journey>, IDomainEvent
{
    public JourneyUpdatedEvent(Journey journey) => Entity = journey;
    public Journey Entity { get; }
}
