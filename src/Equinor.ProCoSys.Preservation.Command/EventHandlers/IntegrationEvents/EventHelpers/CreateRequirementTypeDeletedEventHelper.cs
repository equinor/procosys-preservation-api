using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateRequirementTypeDeletedEventHelper
{
    public static RequirementTypeDeleteEvent CreateEvent(RequirementType entity) => new(entity.Guid, entity.Plant);
}
