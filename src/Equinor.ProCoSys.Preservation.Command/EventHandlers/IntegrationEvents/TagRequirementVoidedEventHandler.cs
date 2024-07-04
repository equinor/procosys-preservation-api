using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class TagRequirementVoidedEventHandler  : INotificationHandler<TagRequirementVoidedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper _createEventHelper;

    public TagRequirementVoidedEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(TagRequirementVoidedEvent notification, CancellationToken cancellationToken)
    {
        var actionEvent = await _createEventHelper.CreateRequirementEvent(notification.TagRequirement);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }

}
