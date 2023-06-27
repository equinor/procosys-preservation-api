using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class RescheduledEvent : IDomainEvent
    {
        public RescheduledEvent(
            string plant,
            Guid sourceGuid,
            int weeks,
            RescheduledDirection direction,
            string comment)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            Weeks = weeks;
            Direction = direction;
            Comment = comment;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public int Weeks { get; }
        public RescheduledDirection Direction { get; }
        public string Comment { get; }
    }
}
