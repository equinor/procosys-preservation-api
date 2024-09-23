using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class VoidTagInSourceEventHandler : INotificationHandler<TagVoidedInSourceEvent>
{
    private readonly ICreateEventHelper<Tag> _createEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public VoidTagInSourceEventHandler(ICreateEventHelper<Tag> createEventHelper, IIntegrationEventPublisher integrationEventPublisher)
    {
        _createEventHelper = createEventHelper;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task Handle(TagVoidedInSourceEvent notification, CancellationToken cancellationToken)
    {
        var tagEvent = await _createEventHelper.CreateEvent(notification.Tag);
        await _integrationEventPublisher.PublishAsync(tagEvent, cancellationToken);
    }
}
