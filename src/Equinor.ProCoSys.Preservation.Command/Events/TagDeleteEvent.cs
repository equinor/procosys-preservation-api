using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[TagEntityName]
public class TagDeleteEvent : DeleteEvent
{
    public TagDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName)
    {
        
    }
}
