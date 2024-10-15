using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ModeDeletedEvent : IPlantEntityEvent<Mode>, IDomainEvent
{
    public ModeDeletedEvent(Mode mode) => Entity = mode;
    public Mode Entity { get; }
}
