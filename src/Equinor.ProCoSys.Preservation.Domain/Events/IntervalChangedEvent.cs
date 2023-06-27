using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class IntervalChangedEvent : IDomainEvent
    {
        public IntervalChangedEvent(
            string plant,
            Guid sourceGuid,
            int requirementDefinitionId,
            int fromInterval,
            int toInterval)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            RequirementDefinitionId = requirementDefinitionId;
            FromInterval = fromInterval;
            ToInterval = toInterval;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public int RequirementDefinitionId { get; }
        public int FromInterval { get; }
        public int ToInterval { get; }
    }
}
