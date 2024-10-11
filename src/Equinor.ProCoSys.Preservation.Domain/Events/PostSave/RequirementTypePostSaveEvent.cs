using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class RequirementTypePostSaveEvent : IPlantEntityEvent<RequirementType>, IPostSaveDomainEvent
{
    public RequirementTypePostSaveEvent(RequirementType entity)
    {
        Entity = entity;
    }

    public RequirementType Entity { get; }
}
