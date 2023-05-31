using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class RescheduledEvent : DomainEvent
    {
        public RescheduledEvent(
            string plant,
            Guid objectGuid,
            int weeks,
            RescheduledDirection direction,
            string comment) : base("Rescheduled")
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
