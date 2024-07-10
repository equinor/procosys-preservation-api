using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ResponsibleUpdatedEvent : IDomainEvent
{
    public ResponsibleUpdatedEvent(Responsible responsible) => Responsible = responsible;
    public Responsible Responsible { get; }
}
