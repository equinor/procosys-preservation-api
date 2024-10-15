using System;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[EntityNameTagRequirement]
public class TagRequirementDeleteEvent : DeleteEvent<TagRequirement>
{
    public TagRequirementDeleteEvent(Guid guid, string plant, string projectName) : base(guid, plant, projectName)
    {
        
    }
}
