using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class AddJourneyEventHandler : INotificationHandler<JourneyAddedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<Journey> _createEventHelper;

    public AddJourneyEventHandler(IIntegrationEventPublisher integrationEventPublisher,
        ICreateEventHelper<Journey> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(JourneyAddedEvent notification, CancellationToken cancellationToken)
    {
        var actionEvent = await _createEventHelper.CreateEvent(notification.Journey);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }
}
