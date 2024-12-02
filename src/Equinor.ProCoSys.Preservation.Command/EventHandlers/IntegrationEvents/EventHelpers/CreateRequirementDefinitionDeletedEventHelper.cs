using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateRequirementDefinitionDeletedEventHelper
{
    public static RequirementDefinitionDeleteEvent CreateEvent(RequirementDefinition entity) => new(entity.Guid, entity.Plant);
}
