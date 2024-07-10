using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class AddRequirementDefinitionEventHandler : INotificationHandler<RequirementDefinitionAddedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper _createEventHelper;

    public AddRequirementDefinitionEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementDefinitionAddedEvent notification, CancellationToken cancellationToken)
    {
        var addedEvent = await _createEventHelper.CreateRequirementDefinitionEvent(notification.RequirementDefinition);
        await _integrationEventPublisher.PublishAsync(addedEvent, cancellationToken);
    }
}
