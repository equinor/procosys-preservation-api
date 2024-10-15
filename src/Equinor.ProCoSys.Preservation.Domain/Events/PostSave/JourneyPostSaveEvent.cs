using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class JourneyPostSaveEvent : IPlantEntityEvent<Journey>, IPostSaveDomainEvent
{
    public JourneyPostSaveEvent(Journey entity)
    {
        Entity = entity;
    }

    public Journey Entity { get; }
}
