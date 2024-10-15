using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateRequirementFieldDeleteEventHelper : ICreateEventHelper<Field, RequirementFieldDeleteEvent>
{
    public Task<RequirementFieldDeleteEvent> CreateEvent(Field entity) => Task.FromResult(new RequirementFieldDeleteEvent(entity.Guid, entity.Plant));
}
