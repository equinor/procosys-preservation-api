using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateResponsibleDeleteEventHelper : ICreateEventHelper<Responsible, ResponsibleDeleteEvent>
{
    public Task<ResponsibleDeleteEvent> CreateEvent(Responsible entity) => Task.FromResult(new ResponsibleDeleteEvent(entity.Guid, entity.Plant));
}
