using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class CloseActionEventHandler : INotificationHandler<ActionClosedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<Action> _createEventHelper;

    public CloseActionEventHandler(IIntegrationEventPublisher integrationEventPublisher,
        ICreateEventHelper<Action> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(ActionClosedEvent notification, CancellationToken cancellationToken)
    {
        var actionEvent = await _createEventHelper.CreateEvent(notification.Action);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }
}
