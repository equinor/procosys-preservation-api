using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagRequirementDeletedEvent : IDomainEvent
    {
        public TagRequirementDeletedEvent(string plant, Guid sourceGuid, int requirementDefinitionId)
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
