using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateModeDeletedEventHelper : ICreateEventHelper<Mode, ModeDeleteEvent>
{
    public Task<ModeDeleteEvent> CreateEvent(Mode entity) => Task.FromResult(new ModeDeleteEvent(entity.Guid, entity.Plant));
}
