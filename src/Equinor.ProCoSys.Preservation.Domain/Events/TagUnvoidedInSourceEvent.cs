using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagUnvoidedInSourceEvent : DomainEvent
    {
        public TagUnvoidedInSourceEvent(
            string plant,
            Guid sourceGuid) : base("Tag unvoided in source")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
