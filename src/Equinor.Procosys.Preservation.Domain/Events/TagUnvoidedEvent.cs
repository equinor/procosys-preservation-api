using System;
using MediatR;

namespace Equinor.Procosys.Preservation.Domain.Events
{
    public class TagUnvoidedEvent : INotification
    {
        public TagUnvoidedEvent(
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
