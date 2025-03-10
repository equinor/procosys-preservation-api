using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers.EventCollections;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagDeleteEventHelper(IProjectRepository projectRepository) : ICreateTagDeleteEventHelper
{
    public async Task<EventCollectionDeleteTag> CreateEvents(Tag entity)
    {
        var project = await projectRepository.GetProjectOnlyByTagGuidAsync(entity.Guid);
        
        var tagDeleteEvent = new TagDeleteEvent(entity.Guid, entity.Plant, project.Name);
        var actionDeleteEvents = entity.Actions.Select(a => CreateActionDeletedEventHelper.CreateEvent(a, project));
        var tagRequirementEvents = entity.Requirements.Select(r => CreateTagRequirementDeleteEventHelper.CreateEvents(r, project));

        return new EventCollectionDeleteTag(tagDeleteEvent, actionDeleteEvents, tagRequirementEvents);
    }
}
