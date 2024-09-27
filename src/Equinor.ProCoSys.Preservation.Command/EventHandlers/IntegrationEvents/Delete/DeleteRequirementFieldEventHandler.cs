using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Delete;

public class DeleteRequirementFieldEventHandler  : INotificationHandler<RequirementDeletedFieldEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    public DeleteRequirementFieldEventHandler(IIntegrationEventPublisher integrationEventPublisher) => _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(RequirementDeletedFieldEvent notification, CancellationToken cancellationToken)
    {
        var deleteEvent = new RequirementFieldDeleteEvent(notification.Entity.Guid, notification.Entity.Plant);
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
