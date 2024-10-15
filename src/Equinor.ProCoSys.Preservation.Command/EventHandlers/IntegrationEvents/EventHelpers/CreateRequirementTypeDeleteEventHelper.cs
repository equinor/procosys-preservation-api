using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateRequirementTypeDeleteEventHelper : ICreateEventHelper<RequirementType, RequirementTypeDeleteEvent>
{
    public Task<RequirementTypeDeleteEvent> CreateEvent(RequirementType entity) => Task.FromResult(new RequirementTypeDeleteEvent(entity.Guid, entity.Plant));
}
