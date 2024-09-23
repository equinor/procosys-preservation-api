using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class AddPreservationPeriodEventHandler : INotificationHandler<PreservationPeriodAddedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<PreservationPeriod> _createEventHelper;

    public AddPreservationPeriodEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper<PreservationPeriod> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(PreservationPeriodAddedEvent notification, CancellationToken cancellationToken)
    {
        var preservationPeriodEvent = await _createEventHelper.CreateEvent(notification.PreservationPeriod);
        await _integrationEventPublisher.PublishAsync(preservationPeriodEvent, cancellationToken);
    }
}
