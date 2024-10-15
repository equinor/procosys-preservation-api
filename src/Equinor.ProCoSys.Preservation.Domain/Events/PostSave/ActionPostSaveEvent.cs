using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class ActionPostSaveEvent : IPlantEntityEvent<Action>, IPostSaveDomainEvent
{
    public ActionPostSaveEvent(Action entity)
    {
        Entity = entity;
    }

    public Action Entity { get; }
}
