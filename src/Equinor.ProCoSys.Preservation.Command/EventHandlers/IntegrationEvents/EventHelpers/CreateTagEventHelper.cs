using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagEventHelper  : ICreateEventHelper<Tag, TagEvent>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICreateChildEventHelper<Project, Tag, TagEvent> _createEventHelper;

    public CreateTagEventHelper(IProjectRepository projectRepository, ICreateChildEventHelper<Project, Tag, TagEvent> createEventHelper)
    {
        _projectRepository = projectRepository;
        _createEventHelper = createEventHelper;
    }

    public async Task<TagEvent> CreateEvent(Tag entity)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(entity.Guid);
        
        return await _createEventHelper.CreateEvent(project, entity);
    }
}
