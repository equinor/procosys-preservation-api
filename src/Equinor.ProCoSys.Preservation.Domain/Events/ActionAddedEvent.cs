using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionAddedEvent : IDomainEvent
    {
        public ActionAddedEvent(string plant, Guid sourceGuid, Guid actionGuid, string title)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            Title = title;
            ActionGuid = actionGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public Guid ActionGuid { get; }
        public string Title { get; }
    }
}
