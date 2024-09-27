using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[ActionEntityName]
public class ActionDeleteEvent : DeleteEvent
{
    public ActionDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName) {}
}