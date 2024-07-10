using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ResponsibleDeletedEvent : IDomainEvent
{
    public ResponsibleDeletedEvent(Responsible responsible) => Responsible = responsible;
    public Responsible Responsible { get; }
}
