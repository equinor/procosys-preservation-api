using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementPreservedEvent : DomainEvent
    {
        public TagRequirementPreservedEvent(
            string plant,
            Guid objectGuid,
            int requirementDefinitionId,
            int? dueInWeeks,
            Guid preservationRecordGuid) : base("Requirement preserved")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            RequirementDefinitionId = requirementDefinitionId;
            DueInWeeks = dueInWeeks;
            PreservationRecordGuid = preservationRecordGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public int RequirementDefinitionId { get; }
        public int? DueInWeeks { get; }
        public Guid PreservationRecordGuid { get; }
    }
}
