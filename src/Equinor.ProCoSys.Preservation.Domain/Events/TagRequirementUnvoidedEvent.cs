using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementUnvoidedEvent : IPlantEntityEvent<TagRequirement>, IDomainEvent
    {
        public TagRequirementUnvoidedEvent(string plant, Guid sourceGuid, TagRequirement tagRequirement)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            Entity = tagRequirement;
        }

        public string Plant { get; }
        public Guid SourceGuid { get; }
        public TagRequirement Entity { get; }
    }
}
