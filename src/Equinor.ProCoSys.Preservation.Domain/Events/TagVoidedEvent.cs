using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagVoidedEvent : DomainEvent
    {
        public TagVoidedEvent(
            string plant,
            Guid objectGuid) : base("Tag voided")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
