using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class TagPostSaveEvent : IPlantEntityEvent<Tag>, IPostSaveDomainEvent
{
    public TagPostSaveEvent(Tag entity)
    {
        Entity = entity;
    }

    public Tag Entity { get; }
}
