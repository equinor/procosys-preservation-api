using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TransferredManuallyEvent : DomainEvent
    {
        public TransferredManuallyEvent(
            string plant,
            Guid objectGuid,
            string fromStep,
            string toStep) : base("Transferred manually")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            FromStep = fromStep;
            ToStep = toStep;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public string FromStep { get; }
        public string ToStep { get; }
    }
}
