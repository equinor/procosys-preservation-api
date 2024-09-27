using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Delete;

public class DeleteResponsibleEventHandler  : INotificationHandler<ResponsibleDeletedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    public DeleteResponsibleEventHandler(IIntegrationEventPublisher integrationEventPublisher) => _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(ResponsibleDeletedEvent notification, CancellationToken cancellationToken)
    {
        var deleteEvent = new ResponsibleDeleteEvent(notification.Entity.Guid, notification.Entity.Plant);
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
