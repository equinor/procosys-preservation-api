using System;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class IntervalChangedEvent : INotification
    {
        public IntervalChangedEvent(
            string plant,
            Guid objectGuid,
            int fromInterval,
            int toInterval)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            FromInterval = fromInterval;
            ToInterval = toInterval;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public int FromInterval { get; }
        public int ToInterval { get; }
    }
}
