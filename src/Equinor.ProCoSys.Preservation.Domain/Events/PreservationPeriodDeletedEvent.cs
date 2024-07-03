using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class PreservationPeriodDeletedEvent : IDomainEvent
{
    public PreservationPeriodDeletedEvent(PreservationPeriod preservationPeriod) => PreservationPeriod = preservationPeriod;

    public PreservationPeriod PreservationPeriod { get; }
}
