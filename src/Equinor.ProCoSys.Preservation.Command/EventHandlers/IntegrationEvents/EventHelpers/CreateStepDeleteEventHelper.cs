using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateStepDeleteEventHelper : ICreateEventHelper<Step, StepDeleteEvent>
{
    public Task<StepDeleteEvent> CreateEvent(Step entity) => Task.FromResult(new StepDeleteEvent(entity.Guid, entity.Plant));
}
