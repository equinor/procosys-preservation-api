using System;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagDeletedInSourceEvent : INotification
    {
        public TagDeletedInSourceEvent(
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
