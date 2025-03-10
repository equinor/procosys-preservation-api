using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Command.Events;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers.EventCollections;

public record EventCollectionDeleteTag(
    TagDeleteEvent TagDeleteEvent,
    IEnumerable<ActionDeleteEvent> ActionDeleteEvents,
    IEnumerable<EventCollectionDeleteTagRequirement> TagRequirementDeleteEvents);
