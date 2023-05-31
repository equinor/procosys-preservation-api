using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagUnvoidedInSourceEvent : DomainEvent
    {
        public TagUnvoidedInSourceEvent(
            string plant,
            Guid objectGuid) : base("Tag unvoided in source")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
