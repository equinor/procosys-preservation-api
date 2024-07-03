using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class PreservationPeriodAddedEvent : IDomainEvent
{
    public PreservationPeriodAddedEvent(PreservationPeriod preservationPeriod) => PreservationPeriod = preservationPeriod;

    public PreservationPeriod PreservationPeriod { get; }
}
