using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class UndoPreservationStartedEvent : DomainEvent
    {
        public UndoPreservationStartedEvent(
            string plant,
            Guid objectGuid) : base("Undo \"Preservation started\"")
        {
            Plant = plant;
            ObjectGuid = objectGuid;
        }
        public string Plant { get; }
        public Guid ObjectGuid { get; }
    }
}
