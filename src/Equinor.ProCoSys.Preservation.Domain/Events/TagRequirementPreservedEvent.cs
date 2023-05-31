using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementPreservedEvent : DomainEvent
    {
        public TagRequirementPreservedEvent(
            string plant,
            Guid sourceGuid,
            int requirementDefinitionId,
            int? dueInWeeks,
            Guid preservationRecordGuid) : base("Requirement preserved")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            RequirementDefinitionId = requirementDefinitionId;
            DueInWeeks = dueInWeeks;
            PreservationRecordGuid = preservationRecordGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public int RequirementDefinitionId { get; }
        public int? DueInWeeks { get; }
        public Guid PreservationRecordGuid { get; }
    }
}
