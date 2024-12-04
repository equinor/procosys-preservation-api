using System.Linq;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateJourneyDeletedEventHelper
{
    public static JourneyDeletedEvents CreateEvent(Journey entity)
    {
        var journeyDeletedEvent = new JourneyDeleteEvent(entity.Guid, entity.Plant);
        var stepDeletedEvents = entity.Steps.Select(CreateStepDeletedEventHelper.CreateEvent).ToList();
        
        return new JourneyDeletedEvents(journeyDeletedEvent, stepDeletedEvents);
    }
}
