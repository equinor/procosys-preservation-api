using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementPreservedEvent : IPlantEntityEvent<TagRequirement>, IDomainEvent
    {
        public TagRequirementPreservedEvent(
            string plant,
            Guid sourceGuid,
            TagRequirement tagRequirement,
            int? dueInWeeks,
            Guid preservationRecordGuid)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            Entity = tagRequirement;
            DueInWeeks = dueInWeeks;
            PreservationRecordGuid = preservationRecordGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public TagRequirement Entity { get; }
        public int? DueInWeeks { get; }
        public Guid PreservationRecordGuid { get; }
    }
}
