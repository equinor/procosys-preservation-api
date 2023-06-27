using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class StepChangedEvent : IDomainEvent
    {
        public StepChangedEvent(
            string plant,
            Guid sourceGuid,
            int fromStepId,
            int toStepId)
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
