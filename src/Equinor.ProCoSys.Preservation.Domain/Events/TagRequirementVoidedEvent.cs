using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementVoidedEvent : DomainEvent
    {
        public TagRequirementVoidedEvent(
            string plant,
            Guid sourceGuid,
            int requirementDefinitionId) : base("Requirement voided")
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
