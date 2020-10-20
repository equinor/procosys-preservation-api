using System;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class RescheduledEvent : INotification
    {
        public RescheduledEvent(
            string plant,
            Guid objectGuid,
            int weeks,
            RescheduledDirection direction)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            Weeks = weeks;
            Direction = direction;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public int Weeks { get; }
        public RescheduledDirection Direction { get; }
    }
}
