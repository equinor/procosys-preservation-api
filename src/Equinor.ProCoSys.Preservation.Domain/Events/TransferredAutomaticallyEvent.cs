using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TransferredAutomaticallyEvent : DomainEvent
    {
        public TransferredAutomaticallyEvent(
            string plant,
            Guid sourceGuid,
            string fromStep,
            string toStep,
            AutoTransferMethod autoTransferMethod) : base("Transferred automatically")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            FromStep = fromStep;
            ToStep = toStep;
            AutoTransferMethod = autoTransferMethod;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public string FromStep { get; }
        public string ToStep { get; }
        public AutoTransferMethod AutoTransferMethod { get; }
    }
}
