using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class PreservationPeriodAddedEvent : IPlantEntityEvent<PreservationPeriod>, IPostSaveDomainEvent
{
    public PreservationPeriodAddedEvent(PreservationPeriod preservationPeriod) => Entity = preservationPeriod;

    public PreservationPeriod Entity { get; }
}
