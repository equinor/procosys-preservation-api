using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateRequirementFieldDeleteEventHelper : ICreateEventHelper<Field, FieldDeleteEvent>
{
    public Task<FieldDeleteEvent> CreateEvent(Field entity) => Task.FromResult(new FieldDeleteEvent(entity.Guid, entity.Plant));
}
