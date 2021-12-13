using System;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class UndoPreservationStartedEvent : INotification
    {
        public UndoPreservationStartedEvent(
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
