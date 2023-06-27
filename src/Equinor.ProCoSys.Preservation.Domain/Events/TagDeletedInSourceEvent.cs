using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagDeletedInSourceEvent : IDomainEvent
    {
        public TagDeletedInSourceEvent(string plant, Guid sourceGuid)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
