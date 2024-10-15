using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class JourneyDeletedEvent : IPlantEntityEvent<Journey>, IDomainEvent
{
    public JourneyDeletedEvent(Journey journey) => Entity = journey;
    public Journey Entity { get; }
}
