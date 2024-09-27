using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[RequirementDefinitionEntityName]
public class RequirementDefinitionDeleteEvent : DeleteEvent
{
    public RequirementDefinitionDeleteEvent(Guid guid, string plant) : base(guid, plant, null) {}
}