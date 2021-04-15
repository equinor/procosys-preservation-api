using System;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class RescheduledEvent : INotification
    {
        public RescheduledEvent(
            string plant,
            Guid objectGuid,
            int weeks,
            RescheduledDirection direction,
            string comment)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            Weeks = weeks;
            Direction = direction;
            Comment = comment;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public int Weeks { get; }
        public RescheduledDirection Direction { get; }
        public string Comment { get; }
    }
}
