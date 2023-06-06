using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementDeletedEvent : DomainEvent
    {
        public TagRequirementDeletedEvent(
            string plant,
            Guid sourceGuid,
            int requirementDefinitionId) : base("Requirement deleted")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            RequirementDefinitionId = requirementDefinitionId;
        }

        public string Plant { get; }
        public Guid SourceGuid { get; }
        public int RequirementDefinitionId { get; }
    }
}
