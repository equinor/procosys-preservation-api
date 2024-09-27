using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[ModeEntityName]
public class ModeDeleteEvent : DeleteEvent
{
    public ModeDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}