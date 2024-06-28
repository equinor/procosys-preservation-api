using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionClosedEvent : IDomainEvent
    {
        public ActionClosedEvent(string plant, Guid sourceGuid, Guid actionGuid)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            ActionGuid = actionGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public Guid ActionGuid { get; }
    }
}
