using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagUnvoidedEvent : DomainEvent
    {
        public TagUnvoidedEvent(
            string plant,
            Guid objectGuid) : base("Tag unvoided")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
