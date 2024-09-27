using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Delete;

public class DeleteStepEventHandler  : INotificationHandler<StepDeletedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    public DeleteStepEventHandler(IIntegrationEventPublisher integrationEventPublisher) => _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(StepDeletedEvent notification, CancellationToken cancellationToken)
    {
        var deleteEvent = new StepDeleteEvent(notification.Entity.Guid, notification.Entity.Plant);
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
