using System;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events
{
    public class UndoPreservationStartedEvent : DomainEvent
    {
        public UndoPreservationStartedEvent(
            string plant,
            Guid sourceGuid) : base("Undo \"Preservation started\"")
        {
            Plant = plant;
            SourceGuid = sourceGuid;
        }
        public string Plant { get; }
        public Guid SourceGuid { get; }
    }
}
