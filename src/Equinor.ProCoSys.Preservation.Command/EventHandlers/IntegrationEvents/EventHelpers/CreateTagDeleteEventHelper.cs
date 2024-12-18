﻿using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagDeleteEventHelper(IProjectRepository projectRepository) : ICreateTagDeleteEventHelper
{
    public async Task<TagDeleteEvents> CreateEvents(Tag entity)
    {
        var project = await projectRepository.GetProjectOnlyByTagGuidAsync(entity.Guid);
        
        var tagDeleteEvent = new TagDeleteEvent(entity.Guid, entity.Plant, project.Name);
        var actionDeleteEvents = entity.Actions.Select(a => CreateActionDeletedEventHelper.CreateEvent(a, project));

        return new TagDeleteEvents(tagDeleteEvent, actionDeleteEvents);
    }
}
