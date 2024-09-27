using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[RequirementTypeEntityName]
public class RequirementTypeDeleteEvent : DeleteEvent
{
    public RequirementTypeDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}