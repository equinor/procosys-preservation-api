using System;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class ActionAddedEvent : INotification
    {
        public ActionAddedEvent(
            string plant,
            Guid objectGuid,
            string title)
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
