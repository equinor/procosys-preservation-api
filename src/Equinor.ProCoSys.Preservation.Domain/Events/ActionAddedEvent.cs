using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionAddedEvent : DomainEvent
    {
        public ActionAddedEvent(
            string plant,
            Guid sourceGuid,
            string title) : base("Action added")
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
