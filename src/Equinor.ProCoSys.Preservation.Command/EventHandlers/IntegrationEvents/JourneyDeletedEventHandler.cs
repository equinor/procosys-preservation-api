using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class JourneyDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : INotificationHandler<DeletedEvent<Journey>>
{
    public async Task Handle(DeletedEvent<Journey> notification, CancellationToken cancellationToken)
    {
        var journeyDeleteEvents = CreateJourneyDeletedEventHelper.CreateEvent(notification.Entity);
        await integrationEventPublisher.PublishAsync(journeyDeleteEvents.JourneyDeleteEvent, cancellationToken);

        foreach (var stepDeleteEvent in journeyDeleteEvents.StepDeleteEvents)
        {
            await integrationEventPublisher.PublishAsync(stepDeleteEvent, cancellationToken);
        }
    }
}
