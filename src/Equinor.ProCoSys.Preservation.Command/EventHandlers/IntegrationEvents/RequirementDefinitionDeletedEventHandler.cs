using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementDefinitionDeletedEventHandler  : INotificationHandler<RequirementDefinitionDeletedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    public RequirementDefinitionDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher) => _integrationEventPublisher = integrationEventPublisher;

    public async Task Handle(RequirementDefinitionDeletedEvent notification, CancellationToken cancellationToken)
    {
        var deleteEvent = new DeleteEvent.RequirementDefinitionDeleteEvent(notification.RequirementDefinition.Guid, notification.RequirementDefinition.Plant);
        await _integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
