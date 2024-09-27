using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateRequirementDefinitionDeletedEventHelper : ICreateEventHelper<RequirementDefinition, RequirementDefinitionDeleteEvent>
{
    public Task<RequirementDefinitionDeleteEvent> CreateEvent(RequirementDefinition entity) => Task.FromResult(new RequirementDefinitionDeleteEvent(entity.Guid, entity.Plant));
}
