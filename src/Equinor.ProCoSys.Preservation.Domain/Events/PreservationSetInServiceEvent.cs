using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class PreservationSetInServiceEvent : DomainEvent
    {
        public PreservationSetInServiceEvent(
            string plant,
            Guid sourceGuid) : base("Set in service")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
