using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateStepDeletedEventHelper
{
    public static StepDeleteEvent CreateEvent(Step entity) => new(entity.Guid, entity.Plant);
}
