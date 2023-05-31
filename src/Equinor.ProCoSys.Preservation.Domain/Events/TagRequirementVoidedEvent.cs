using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementVoidedEvent : DomainEvent
    {
        public TagRequirementVoidedEvent(
            string plant,
            Guid objectGuid,
            int requirementDefinitionId) : base("Requirement voided")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            RequirementDefinitionId = requirementDefinitionId;
        }

        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public int RequirementDefinitionId { get; }
    }
}
