using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementAddedEvent : DomainEvent
    {
        public TagRequirementAddedEvent(
            string plant,
            Guid objectGuid,
            int requirementDefinitionId) : base("Tag requirement added")
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
