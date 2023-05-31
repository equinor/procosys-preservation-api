using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagUnvoidedEvent : DomainEvent
    {
        public TagUnvoidedEvent(
            string plant,
            Guid sourceGuid) : base("Tag unvoided")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
