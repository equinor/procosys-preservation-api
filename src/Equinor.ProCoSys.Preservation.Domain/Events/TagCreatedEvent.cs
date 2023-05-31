using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagCreatedEvent : DomainEvent
    {
        public TagCreatedEvent(
            string plant,
            Guid objectGuid) : base("Tag created")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
