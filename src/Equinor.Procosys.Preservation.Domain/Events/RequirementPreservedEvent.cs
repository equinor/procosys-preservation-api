using System;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class RequirementPreservedEvent : INotification
    {
        public RequirementPreservedEvent(
            string plant,
            Guid objectGuid,
            int requirementDefinitionId,
            int? nextDueInWeeks,
            Guid preservationRecordGuid)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            RequirementDefinitionId = requirementDefinitionId;
            NextDueInWeeks = nextDueInWeeks;
            PreservationRecordGuid = preservationRecordGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public int RequirementDefinitionId { get; }
        public int? NextDueInWeeks { get; }
        public Guid PreservationRecordGuid { get; }
    }
}
