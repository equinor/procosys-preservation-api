using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class StepChangedEvent : DomainEvent
    {
        public StepChangedEvent(
            string plant,
            Guid sourceGuid,
            int fromStepId,
            int toStepId) : base("Step changed")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            FromStepId = fromStepId;
            ToStepId = toStepId;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public int FromStepId { get; }
        public int ToStepId { get; }
    }
}
