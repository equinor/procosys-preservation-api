using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class JourneyUpdatedEvent : IDomainEvent
{
    public JourneyUpdatedEvent(Journey journey) => Journey = journey;
    public Journey Journey { get; }
}
