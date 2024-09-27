using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[TagRequirementEntityName]
public class TagRequirementDeleteEvent : DeleteEvent
{
    public TagRequirementDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName)
    {
        
    }
}
