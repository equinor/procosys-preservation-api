using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Command.Events;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers.EventCollections;

public record EventCollectionDeleteTagRequirement(
    TagRequirementDeleteEvent TagRequirementDeleteEvent,
    IEnumerable<PreservationPeriodDeleteEvent> PreservationPeriodDeleteEvents);
