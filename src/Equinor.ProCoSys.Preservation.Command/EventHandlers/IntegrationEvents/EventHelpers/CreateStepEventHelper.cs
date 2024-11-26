using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateStepEventHelper(
    IJourneyRepository journeyRepository,
    ICreateChildEventHelper<Journey, Step, StepEvent> createActionEventHelper)
    : ICreateEventHelper<Step, StepEvent>
{
    public async Task<StepEvent> CreateEvent(Step entity)
    {
        var journeys = await journeyRepository.GetJourneysByStepIdsAsync([entity.Id]);
        
        return await createActionEventHelper.CreateEvent(journeys.First(), entity);
    }
}
