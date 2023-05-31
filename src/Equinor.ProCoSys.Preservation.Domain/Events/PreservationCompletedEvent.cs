using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class PreservationCompletedEvent : DomainEvent
    {
        public PreservationCompletedEvent(
            string plant,
            Guid objectGuid) : base("Preservation completed")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
