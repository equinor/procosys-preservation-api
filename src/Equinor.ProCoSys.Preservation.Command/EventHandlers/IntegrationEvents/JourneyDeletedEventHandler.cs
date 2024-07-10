﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class JourneyDeletedEventHandler  : INotificationHandler<JourneyDeletedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    public JourneyDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher) => _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(JourneyDeletedEvent notification, CancellationToken cancellationToken)
    {
        var deleteEvent = new DeleteEvent.JourneyDeleteEvent(notification.Journey.Guid, notification.Journey.Plant);
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
