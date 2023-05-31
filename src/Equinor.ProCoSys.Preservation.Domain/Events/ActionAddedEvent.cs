using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionAddedEvent : DomainEvent
    {
        public ActionAddedEvent(
            string plant,
            Guid objectGuid,
            string title) : base("Action added")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
            Title = title;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
        public string Title { get; }
    }
}
