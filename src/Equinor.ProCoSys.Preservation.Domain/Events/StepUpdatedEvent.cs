using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class StepUpdatedEvent : IPlantEntityEvent<Step>, IDomainEvent
{
    public StepUpdatedEvent(Step step) => Entity = step;
    public Step Entity { get; }
}
