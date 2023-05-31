using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class StepChangedEvent : DomainEvent
    {
        public StepChangedEvent(
            string plant,
            Guid objectGuid,
            int fromStepId,
            int toStepId) : base("Step changed")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            FromStepId = fromStepId;
            ToStepId = toStepId;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public int FromStepId { get; }
        public int ToStepId { get; }
    }
}
