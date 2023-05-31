using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class TagVoidedEvent : DomainEvent
    {
        public TagVoidedEvent(
            string plant,
            Guid sourceGuid) : base("Tag voided")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
