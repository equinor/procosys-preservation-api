using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagDeletedInSourceEvent : DomainEvent
    {
        public TagDeletedInSourceEvent(
            string plant,
            Guid sourceGuid) : base("Tag deleted in source system")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
