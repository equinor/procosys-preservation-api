using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagDeleteEventHelper : ICreateEventHelper<Tag, TagDeleteEvent>
{
    private readonly IProjectRepository _projectRepository;

    public CreateTagDeleteEventHelper(IProjectRepository projectRepository) => _projectRepository = projectRepository;

    public async Task<TagDeleteEvent> CreateEvent(Tag entity)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(entity.Guid);
        return new TagDeleteEvent(entity.Guid, entity.Plant, project.Name);
    }
}
