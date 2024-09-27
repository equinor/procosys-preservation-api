using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[TagEntityName]
public class TagDeleteEvent : DeleteEvent<Tag>
{
    public TagDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName)
    {
        
    }
}
