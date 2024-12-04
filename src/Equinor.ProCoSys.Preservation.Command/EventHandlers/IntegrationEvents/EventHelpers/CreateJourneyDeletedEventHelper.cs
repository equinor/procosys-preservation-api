using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateJourneyDeletedEventHelper
{
    public static JourneyDeleteEvent CreateEvent(Journey entity) => new(entity.Guid, entity.Plant);
}
