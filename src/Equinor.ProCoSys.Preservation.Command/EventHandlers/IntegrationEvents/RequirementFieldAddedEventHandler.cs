using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementFieldAddedEventHandler : INotificationHandler<RequirementAddedFieldEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper _createEventHelper;

    public RequirementFieldAddedEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementAddedFieldEvent notification, CancellationToken cancellationToken)
    {
        var addedEvent = await _createEventHelper.CreateFieldEvent(notification.Field, notification.RequirementDefinitionGuid);
        await _integrationEventPublisher.PublishAsync(addedEvent, cancellationToken);
    }
}
