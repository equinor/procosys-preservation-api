using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionClosedEvent : DomainEvent
    {
        public ActionClosedEvent(
            string plant,
            Guid sourceGuid) : base("Action closed")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
