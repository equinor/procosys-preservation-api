using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class TagRequirementPostSaveEvent : IPlantEntityEvent<TagRequirement>, IPostSaveDomainEvent
{
    public TagRequirementPostSaveEvent(TagRequirement entity)
    {
        Entity = entity;
    }

    public TagRequirement Entity { get; }
}
