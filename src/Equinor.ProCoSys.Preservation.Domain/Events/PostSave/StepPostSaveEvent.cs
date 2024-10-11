using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class StepPostSaveEvent : IPlantEntityEvent<Step>, IPostSaveDomainEvent
{
    public StepPostSaveEvent(Step entity)
    {
        Entity = entity;
    }

    public Step Entity { get; }
}
