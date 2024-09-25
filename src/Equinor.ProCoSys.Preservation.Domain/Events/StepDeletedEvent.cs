using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class StepDeletedEvent : IPlantEntityEvent<Step>, IDomainEvent
{
    public StepDeletedEvent(Step step) => Entity = step;
    public Step Entity { get; }
}
