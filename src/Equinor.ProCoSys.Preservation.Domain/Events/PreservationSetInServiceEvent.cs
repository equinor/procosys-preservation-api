using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class PreservationSetInServiceEvent : IDomainEvent
    {
        public PreservationSetInServiceEvent(string plant, Guid sourceGuid)
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
