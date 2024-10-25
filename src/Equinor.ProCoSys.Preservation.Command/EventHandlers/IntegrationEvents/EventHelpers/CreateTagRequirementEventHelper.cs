using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagRequirementEventHelper : ICreateEventHelper<TagRequirement, TagRequirementEvent>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent> _createEventHelper;

    public CreateTagRequirementEventHelper(IProjectRepository projectRepository, ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent> createEventHelper)
    {
        _projectRepository = projectRepository;
        _createEventHelper = createEventHelper;
    }
    
    public async Task<TagRequirementEvent> CreateEvent(TagRequirement entity)
    {
        var tag = await _projectRepository.GetTagByTagRequirementGuidAsync(entity.Guid);
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tag.Guid);
        
        return await _createEventHelper.CreateEvent(project, entity);
    }
}
