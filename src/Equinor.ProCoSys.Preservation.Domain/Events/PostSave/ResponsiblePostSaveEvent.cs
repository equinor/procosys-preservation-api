using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class ResponsiblePostSaveEvent : IPlantEntityEvent<Responsible>, IPostSaveDomainEvent
{
    public ResponsiblePostSaveEvent(Responsible entity)
    {
        Entity = entity;
    }

    public Responsible Entity { get; }
}
