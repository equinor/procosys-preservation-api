using System;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagUnvoidedInSourceEvent : INotification
    {
        public TagUnvoidedInSourceEvent(
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
