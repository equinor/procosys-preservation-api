using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Command.Events;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public record TagRequirementDeleteEvents(
    TagRequirementDeleteEvent TagRequirementDeleteEvent,
    IEnumerable<PreservationPeriodDeleteEvent> PreservationPeriodDeleteEvents);
