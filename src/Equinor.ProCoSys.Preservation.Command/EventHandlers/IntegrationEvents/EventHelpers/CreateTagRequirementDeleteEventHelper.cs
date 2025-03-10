using System.Linq;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers.EventCollections;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public static class CreateTagRequirementDeleteEventHelper
{
    public static EventCollectionDeleteTagRequirement CreateEvents(TagRequirement entity, Project project)
    {
        var tagRequirementDeleteEvent = new TagRequirementDeleteEvent(entity.Guid, entity.Plant, project.Name);
        var preservationPeriodDeleteEvents = entity.PreservationPeriods.Select(p => new PreservationPeriodDeleteEvent(p.Guid, entity.Plant, project.Name));
        
        return new EventCollectionDeleteTagRequirement(tagRequirementDeleteEvent, preservationPeriodDeleteEvents);
    }
}
