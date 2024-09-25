using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class DeleteTagEventHandler  : INotificationHandler<TagDeletedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly IProjectRepository _projectRepository;

    public DeleteTagEventHandler(IIntegrationEventPublisher integrationEventPublisher,
        IProjectRepository projectRepository)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _projectRepository = projectRepository;
    }

    public async Task Handle(TagDeletedEvent notification, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(notification.Entity.Guid);

        var deleteEvent = new DeleteEvent.TagDeleteEvent(notification.Entity.Guid, notification.Entity.Plant, project.Name);
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
