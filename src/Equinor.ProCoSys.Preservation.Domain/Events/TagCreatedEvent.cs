using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagCreatedEvent : DomainEvent
    {
        public TagCreatedEvent(
            string plant,
            Guid sourceGuid) : base("Tag created")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
