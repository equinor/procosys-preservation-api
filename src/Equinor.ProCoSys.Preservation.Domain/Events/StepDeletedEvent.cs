using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class StepDeletedEvent : IDomainEvent
{
    public StepDeletedEvent(Step step) => Step = step;
    public Step Step { get; }
}
