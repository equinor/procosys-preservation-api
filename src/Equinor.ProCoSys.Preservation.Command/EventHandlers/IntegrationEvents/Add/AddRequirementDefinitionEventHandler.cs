using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class AddRequirementDefinitionEventHandler : INotificationHandler<RequirementDefinitionAddedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<RequirementDefinition> _createEventHelper;

    public AddRequirementDefinitionEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper<RequirementDefinition> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementDefinitionAddedEvent notification, CancellationToken cancellationToken)
    {
        var addedEvent = await _createEventHelper.CreateEvent(notification.RequirementDefinition);
        await _integrationEventPublisher.PublishAsync(addedEvent, cancellationToken);
    }
}
