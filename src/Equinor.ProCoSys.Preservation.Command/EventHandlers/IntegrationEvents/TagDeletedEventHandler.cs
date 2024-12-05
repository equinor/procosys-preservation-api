using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class TagDeletedEventHandler(
    ICreateTagDeleteEventHelper createEventHelper,
    IIntegrationEventPublisher integrationEventPublisher)
    : INotificationHandler<DeletedEvent<Tag>>
{
    public async Task Handle(DeletedEvent<Tag> notification, CancellationToken cancellationToken)
    {
        var integrationEvent = await createEventHelper.CreateEvents(notification.Entity);
        await integrationEventPublisher.PublishAsync(integrationEvent.TagDeleteEvent, cancellationToken);
    }
}
