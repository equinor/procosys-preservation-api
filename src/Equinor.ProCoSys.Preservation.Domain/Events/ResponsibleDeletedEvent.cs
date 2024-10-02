using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ResponsibleDeletedEvent : IPlantEntityEvent<Responsible>, IDomainEvent
{
    public ResponsibleDeletedEvent(Responsible responsible) => Entity = responsible;
    public Responsible Entity { get; }
}
