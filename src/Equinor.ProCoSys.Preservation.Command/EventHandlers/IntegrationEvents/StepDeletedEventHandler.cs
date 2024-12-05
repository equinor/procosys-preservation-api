using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class StepDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : INotificationHandler<DeletedEvent<Step>>
{
    public async Task Handle(DeletedEvent<Step> notification, CancellationToken cancellationToken)
    {
        var deleteEvent = CreateStepDeletedEventHelper.CreateEvent(notification.Entity);
        await integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
