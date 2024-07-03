using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class TagRequirementPreservedEventHandler  : INotificationHandler<TagRequirementPreservedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper _createEventHelper;

    public TagRequirementPreservedEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(TagRequirementPreservedEvent notification, CancellationToken cancellationToken)
    {
        var actionEvent = await _createEventHelper.CreateRequirementEvent(notification.TagRequirement, notification.SourceGuid);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }
}
