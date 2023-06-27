using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionAddedEvent : IDomainEvent
    {
        public ActionAddedEvent(string plant, Guid sourceGuid, string title)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
            Title = title;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
        public string Title { get; }
    }
}
