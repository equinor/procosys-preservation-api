using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[StepEntityName]
public class StepDeleteEvent : DeleteEvent
{
    public StepDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}