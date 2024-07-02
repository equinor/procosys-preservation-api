using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementFieldUpdatedEventHandler : INotificationHandler<RequirementUpdatedFieldEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper _createEventHelper;

    public RequirementFieldUpdatedEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementUpdatedFieldEvent notification, CancellationToken cancellationToken)
    {
        var updatedEvent = await _createEventHelper.CreateFieldEvent(notification.Field, notification.RequirementDefinitionGuid);
        await _integrationEventPublisher.PublishAsync(updatedEvent, cancellationToken);
    }
}
