using System;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class PreservationCompletedEvent : INotification
    {
        public PreservationCompletedEvent(
            string plant,
            Guid objectGuid)
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
