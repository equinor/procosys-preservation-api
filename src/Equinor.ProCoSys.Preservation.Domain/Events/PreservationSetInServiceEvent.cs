using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class PreservationSetInServiceEvent : DomainEvent
    {
        public PreservationSetInServiceEvent(
            string plant,
            Guid objectGuid) : base("Set in service")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
