using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateFieldDeleteEventHelper
{
    public static FieldDeleteEvent CreateEvent(Field entity) => new(entity.Guid, entity.Plant);
}
