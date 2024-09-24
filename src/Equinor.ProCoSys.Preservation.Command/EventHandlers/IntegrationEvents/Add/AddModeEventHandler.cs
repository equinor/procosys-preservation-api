using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class AddModeEventHandler : INotificationHandler<ModeAddedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly  ICreateEventHelper<Mode> _createEventHelper;

    public AddModeEventHandler(IIntegrationEventPublisher integrationEventPublisher,
        ICreateEventHelper<Mode> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(ModeAddedEvent notification, CancellationToken cancellationToken)
    {
        var actionEvent = await _createEventHelper.CreateEvent(notification.Mode);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }
}
