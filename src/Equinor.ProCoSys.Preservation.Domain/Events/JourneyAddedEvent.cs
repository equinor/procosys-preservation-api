using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class JourneyAddedEvent : IPlantEntityEvent<Journey>, IPostSaveDomainEvent
{
    public JourneyAddedEvent(Journey journey) => Entity = journey;
    public Journey Entity { get; }
}
