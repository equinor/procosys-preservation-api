using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class ModePostSaveEvent : IPlantEntityEvent<Mode>, IPostSaveDomainEvent
{
    public ModePostSaveEvent(Mode entity)
    {
        Entity = entity;
    }

    public Mode Entity { get; }
}
