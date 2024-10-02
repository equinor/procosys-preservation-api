using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ModeUpdatedEvent : IPlantEntityEvent<Mode>, IDomainEvent
{
    public ModeUpdatedEvent(Mode mode) => Entity = mode;
    public Mode Entity { get; }
}
