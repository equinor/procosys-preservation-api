using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class PublishTagRequirementEventHelper : IPublishEntityEventHelper<TagRequirement>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent> _createEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public PublishTagRequirementEventHelper(
        IProjectRepository projectRepository,
        ICreateChildEventHelper<Project, TagRequirement, TagRequirementEvent> createEventHelper,
        IIntegrationEventPublisher integrationEventPublisher)
    {
        _projectRepository = projectRepository;
        _createEventHelper = createEventHelper;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task PublishEvent(TagRequirement entity, CancellationToken cancellationToken)
    {
        var tag = await _projectRepository.GetTagByTagRequirementGuidAsync(entity.Guid);
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tag.Guid);
        
         var integrationEvent = await _createEventHelper.CreateEvent(project, entity);
         await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
