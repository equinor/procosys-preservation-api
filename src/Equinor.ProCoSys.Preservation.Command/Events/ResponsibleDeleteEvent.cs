using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[ResponsibleEntityName]
public class ResponsibleDeleteEvent : DeleteEvent
{
    public ResponsibleDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}