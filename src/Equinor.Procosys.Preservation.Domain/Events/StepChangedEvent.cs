using System;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class StepChangedEvent : INotification
    {
        public StepChangedEvent(
            string plant,
            Guid objectGuid,
            int fromStepId,
            int toStepId)
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
