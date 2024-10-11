using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class FieldPostSaveEvent : IPlantEntityEvent<Field>, IPostSaveDomainEvent
{
    public FieldPostSaveEvent(Field entity)
    {
        Entity = entity;
    }

    public Field Entity { get; }
}
