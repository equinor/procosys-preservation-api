using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class PreservationPeriodDeletedEvent : IPlantEntityEvent<PreservationPeriod>, IDomainEvent
{
    public PreservationPeriodDeletedEvent(PreservationPeriod preservationPeriod) => Entity = preservationPeriod;

    public PreservationPeriod Entity { get; }
}
