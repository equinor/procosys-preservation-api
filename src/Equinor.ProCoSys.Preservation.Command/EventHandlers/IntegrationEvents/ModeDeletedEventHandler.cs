using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class ModeDeletedEventHandler  : INotificationHandler<ModeDeletedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    public ModeDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher) => _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(ModeDeletedEvent notification, CancellationToken cancellationToken)
    {
        var deleteEvent = new DeleteEvent.ModeDeleteEvent(notification.Mode.Guid, notification.Mode.Plant);
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
