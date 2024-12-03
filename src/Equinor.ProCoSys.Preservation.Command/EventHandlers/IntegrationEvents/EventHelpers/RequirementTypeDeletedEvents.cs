using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Command.Events;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public record RequirementTypeDeletedEvents(
    RequirementTypeDeleteEvent TypeDeleteEvent,
    IEnumerable<RequirementDefinitionDeleteEvent> DefinitionDeleteEvents,
    IEnumerable<FieldDeleteEvent> FieldDeleteEvents);

