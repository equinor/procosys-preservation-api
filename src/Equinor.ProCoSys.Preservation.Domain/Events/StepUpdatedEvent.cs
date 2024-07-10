using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class StepUpdatedEvent : IDomainEvent
{
    public StepUpdatedEvent(Step step) => Step = step;
    public Step Step { get; }
}
