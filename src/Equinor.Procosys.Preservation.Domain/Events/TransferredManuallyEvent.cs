using System;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class TransferredManuallyEvent : INotification
    {
        public TransferredManuallyEvent(
            string plant,
            Guid objectGuid,
            string fromStep,
            string toStep)
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
