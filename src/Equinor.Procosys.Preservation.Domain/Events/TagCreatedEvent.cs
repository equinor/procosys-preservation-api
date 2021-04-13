using System;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagCreatedEvent : INotification
    {
        public TagCreatedEvent(
            string plant,
            Guid objectGuid)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
