using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class FieldDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : INotificationHandler<DeletedEvent<Field>>
{
    public async Task Handle(DeletedEvent<Field> notification, CancellationToken cancellationToken)
    {
        var integrationEvent = CreateFieldDeleteEventHelper.CreateEvent(notification.Entity);
        await integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
