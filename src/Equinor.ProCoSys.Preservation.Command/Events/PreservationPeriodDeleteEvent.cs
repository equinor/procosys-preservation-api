using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[PreservationPeriodEntityName]
public class PreservationPeriodDeleteEvent : DeleteEvent
{
    public PreservationPeriodDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}