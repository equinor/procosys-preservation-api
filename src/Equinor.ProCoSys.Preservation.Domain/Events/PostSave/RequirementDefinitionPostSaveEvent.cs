using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events.PostSave;

public class RequirementDefinitionPostSaveEvent : IPlantEntityEvent<RequirementDefinition>, IPostSaveDomainEvent
{
    public RequirementDefinitionPostSaveEvent(RequirementDefinition entity)
    {
        Entity = entity;
    }

    public RequirementDefinition Entity { get; }
}
