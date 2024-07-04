using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementFieldDeletedEventHandler  : INotificationHandler<RequirementDeletedFieldEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    public RequirementFieldDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher) => _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(RequirementDeletedFieldEvent notification, CancellationToken cancellationToken)
    {
        var deleteEvent = new DeleteEvent.RequirementFieldDeleteEvent(notification.Field.Guid, notification.Field.Plant, "");
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
