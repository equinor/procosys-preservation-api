using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TransferredAutomaticallyEvent : INotification
    {
        public TransferredAutomaticallyEvent(
            string plant,
            Guid objectGuid,
            string fromStep,
            string toStep,
            AutoTransferMethod autoTransferMethod)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            FromStep = fromStep;
            ToStep = toStep;
            AutoTransferMethod = autoTransferMethod;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public string FromStep { get; }
        public string ToStep { get; }
        public AutoTransferMethod AutoTransferMethod { get; }
    }
}
