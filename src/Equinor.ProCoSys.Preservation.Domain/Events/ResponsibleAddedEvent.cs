using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ResponsibleAddedEvent : IPostSaveDomainEvent
{
    public ResponsibleAddedEvent(Responsible responsible) => Responsible = responsible;
    public Responsible Responsible { get; }
}
