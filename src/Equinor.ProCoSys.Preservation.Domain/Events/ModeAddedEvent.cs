using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ModeAddedEvent : IPostSaveDomainEvent
{
    public Mode Mode { get; }
    public ModeAddedEvent(Mode mode) => Mode = mode;
}
