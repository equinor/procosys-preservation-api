using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[RequirementFieldEntityName]
public class RequirementFieldDeleteEvent : DeleteEvent
{
    public RequirementFieldDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}