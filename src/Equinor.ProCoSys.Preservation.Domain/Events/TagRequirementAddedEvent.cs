using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementAddedEvent : IDomainEvent
    {
        public TagRequirementAddedEvent(string plant, TagRequirement tagRequirement)
        {
            Plant = plant;
            TagRequirement = tagRequirement;
        }

        public string Plant { get; }
        public TagRequirement TagRequirement { get; }
    }
}
