using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementAddedEvent : IPlantEntityEvent<TagRequirement>, IDomainEvent
    {
        public TagRequirementAddedEvent(string plant, Guid sourceGouid, TagRequirement tagRequirement)
        {
            Plant = plant;
            SourceGouid = sourceGouid;
            Entity = tagRequirement;
        }

        public string Plant { get; }
        public Guid SourceGouid { get; }
        public TagRequirement Entity { get; }
    }
}
