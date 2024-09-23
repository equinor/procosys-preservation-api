using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class UpdateRequirementDefinitionEventHandler : INotificationHandler<RequirementDefinitionUpdatedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<RequirementDefinition> _createEventHelper;

    public UpdateRequirementDefinitionEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper<RequirementDefinition> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementDefinitionUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var updatedEvent = await _createEventHelper.CreateEvent(notification.RequirementDefinition);
        await _integrationEventPublisher.PublishAsync(updatedEvent, cancellationToken);
    }
}
