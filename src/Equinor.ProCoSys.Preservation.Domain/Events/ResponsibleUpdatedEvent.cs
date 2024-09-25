using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ResponsibleUpdatedEvent : IPlantEntityEvent<Responsible>, IDomainEvent
{
    public ResponsibleUpdatedEvent(Responsible responsible) => Entity = responsible;
    public Responsible Entity { get; }
}
