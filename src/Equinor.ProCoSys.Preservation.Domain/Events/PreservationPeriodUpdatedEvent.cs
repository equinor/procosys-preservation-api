using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class PreservationPeriodUpdatedEvent : IPlantEntityEvent<PreservationPeriod>, IDomainEvent
{
    public PreservationPeriodUpdatedEvent(PreservationPeriod preservationPeriod) => Entity = preservationPeriod;
    public PreservationPeriod Entity { get; }
}
