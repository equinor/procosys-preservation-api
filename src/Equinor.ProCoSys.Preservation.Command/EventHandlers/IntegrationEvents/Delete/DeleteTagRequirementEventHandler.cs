using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class DeleteTagRequirementEventHandler  : INotificationHandler<TagRequirementDeletedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly IProjectRepository _projectRepository;

    public DeleteTagRequirementEventHandler(IIntegrationEventPublisher integrationEventPublisher,
        IProjectRepository projectRepository)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _projectRepository = projectRepository;
    }
    
    public async Task Handle(TagRequirementDeletedEvent notification, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(notification.SourceGuid);

        var deleteEvent = new DeleteEvent.TagRequirementDeleteEvent(notification.Entity.Guid, notification.Plant, project.Name);
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
