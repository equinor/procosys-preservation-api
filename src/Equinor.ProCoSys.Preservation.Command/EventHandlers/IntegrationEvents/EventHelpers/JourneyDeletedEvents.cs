using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Command.Events;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public record JourneyDeletedEvents(
    JourneyDeleteEvent JourneyDeleteEvent,
    IEnumerable<StepDeleteEvent> StepDeleteEvents);

