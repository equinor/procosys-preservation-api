using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class PreservationPeriodPostSaveEvent : IPlantEntityEvent<PreservationPeriod>, IPostSaveDomainEvent
{
    public PreservationPeriodPostSaveEvent(PreservationPeriod entity)
    {
        Entity = entity;
    }

    public PreservationPeriod Entity { get; }
}
