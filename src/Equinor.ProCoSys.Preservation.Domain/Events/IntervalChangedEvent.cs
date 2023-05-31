using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class IntervalChangedEvent : DomainEvent
    {
        public IntervalChangedEvent(
            string plant,
            Guid objectGuid,
            int requirementDefinitionId,
            int fromInterval,
            int toInterval) : base("Interval changed")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            RequirementDefinitionId = requirementDefinitionId;
            FromInterval = fromInterval;
            ToInterval = toInterval;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public int RequirementDefinitionId { get; }
        public int FromInterval { get; }
        public int ToInterval { get; }
    }
}
