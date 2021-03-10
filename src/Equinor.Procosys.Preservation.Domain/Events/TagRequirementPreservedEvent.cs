using System;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class TagRequirementPreservedEvent : INotification
    {
        public TagRequirementPreservedEvent(
            string plant,
            Guid objectGuid,
            int requirementDefinitionId,
            int? dueInWeeks,
            Guid preservationRecordGuid)
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
