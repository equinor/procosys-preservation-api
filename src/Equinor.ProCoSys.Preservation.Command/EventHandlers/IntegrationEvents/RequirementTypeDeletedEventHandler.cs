﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementTypeDeletedEventHandler  : INotificationHandler<RequirementTypeDeletedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    public RequirementTypeDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher) => _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(RequirementTypeDeletedEvent notification, CancellationToken cancellationToken)
    {
        var deleteEvent = new DeleteEvent(notification.RequirementType.Guid, notification.RequirementType.Plant, "");
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}