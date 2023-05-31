using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class PreservationStartedEvent : DomainEvent
    {
        public PreservationStartedEvent(
            string plant,
            Guid objectGuid) : base("Preservation started")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
