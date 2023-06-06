using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class PreservationStartedEvent : DomainEvent
    {
        public PreservationStartedEvent(
            string plant,
            Guid sourceGuid) : base("Preservation started")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
