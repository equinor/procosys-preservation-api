using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementUnvoidedEvent : IDomainEvent
    {
        public TagRequirementUnvoidedEvent(string plant, Guid sourceGuid, TagRequirement tagRequirement)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            TagRequirement = tagRequirement;
        }

        public string Plant { get; }
        public Guid SourceGuid { get; }
        public TagRequirement TagRequirement { get; }
    }
}
