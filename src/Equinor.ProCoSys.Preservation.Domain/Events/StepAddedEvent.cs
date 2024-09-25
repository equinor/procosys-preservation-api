using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class StepAddedEvent : IPlantEntityEvent<Step>, IPostSaveDomainEvent
{
    public StepAddedEvent(Step step) => Entity = step;
    public Step Entity { get; }
}
