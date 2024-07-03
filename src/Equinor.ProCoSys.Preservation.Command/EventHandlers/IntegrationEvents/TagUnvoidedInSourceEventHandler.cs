using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class TagUnvoidedInSourceEventHandler : INotificationHandler<TagUnvoidedInSourceEvent>
{
    private readonly ICreateEventHelper _createEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public TagUnvoidedInSourceEventHandler(ICreateEventHelper createEventHelper, IIntegrationEventPublisher integrationEventPublisher)
    {
        _createEventHelper = createEventHelper;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task Handle(TagUnvoidedInSourceEvent notification, CancellationToken cancellationToken)
    {
        var tagEvent = await _createEventHelper.CreateTagEvent(notification.Tag);
        await _integrationEventPublisher.PublishAsync(tagEvent, cancellationToken);
    }
}
