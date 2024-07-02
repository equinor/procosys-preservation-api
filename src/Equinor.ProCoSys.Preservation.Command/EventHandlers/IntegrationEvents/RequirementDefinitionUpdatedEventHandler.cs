using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementDefinitionUpdatedEventHandler : INotificationHandler<RequirementDefinitionUpdatedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper _createEventHelper;

    public RequirementDefinitionUpdatedEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementDefinitionUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var updatedEvent = await _createEventHelper.CreateRequirementDefinitionEvent(notification.RequirementDefinition);
        await _integrationEventPublisher.PublishAsync(updatedEvent, cancellationToken);
    }
}
