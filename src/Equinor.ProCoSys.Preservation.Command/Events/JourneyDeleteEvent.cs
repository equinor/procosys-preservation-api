using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[JourneyEntityName]
public class JourneyDeleteEvent : DeleteEvent
{
    public JourneyDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}