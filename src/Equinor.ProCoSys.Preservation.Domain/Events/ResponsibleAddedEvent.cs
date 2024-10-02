using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ResponsibleAddedEvent : IPlantEntityEvent<Responsible>, IPostSaveDomainEvent
{
    public ResponsibleAddedEvent(Responsible responsible) => Entity = responsible;
    public Responsible Entity { get; }
}
