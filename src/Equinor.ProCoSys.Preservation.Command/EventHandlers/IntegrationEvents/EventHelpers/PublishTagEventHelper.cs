using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class PublishTagEventHelper : IPublishEntityEventHelper<Tag>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICreateProjectEventHelper<Tag, TagEvent> _createEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public PublishTagEventHelper(
        IProjectRepository projectRepository,
        ICreateProjectEventHelper<Tag, TagEvent> createEventHelper,
        IIntegrationEventPublisher integrationEventPublisher)
    {
        _projectRepository = projectRepository;
        _createEventHelper = createEventHelper;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task PublishEvent(Tag entity, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(entity.Guid);
        
         var integrationEvent = await _createEventHelper.CreateEvent(entity, project.Name);
         await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
