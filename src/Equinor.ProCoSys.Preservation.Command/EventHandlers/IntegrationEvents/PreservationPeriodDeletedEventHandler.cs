using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class PreservationPeriodDeletedEventHandler  : INotificationHandler<PreservationPeriodDeletedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    public PreservationPeriodDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher) => _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(PreservationPeriodDeletedEvent notification, CancellationToken cancellationToken)
    {
        var deleteEvent = new DeleteEvent(notification.PreservationPeriod.Guid, notification.PreservationPeriod.Plant, "");
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
