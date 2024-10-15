using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateJourneyDeletedEventHelper : ICreateEventHelper<Journey, JourneyDeleteEvent>
{
    public Task<JourneyDeleteEvent> CreateEvent(Journey entity) => Task.FromResult(new JourneyDeleteEvent(entity.Guid, entity.Plant));
}
