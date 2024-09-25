using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ModeAddedEvent : IPlantEntityEvent<Mode>, IPostSaveDomainEvent
{
    public Mode Entity { get; }
    public ModeAddedEvent(Mode mode) => Entity = mode;
}
