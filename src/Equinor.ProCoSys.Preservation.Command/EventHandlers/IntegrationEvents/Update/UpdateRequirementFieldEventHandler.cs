using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class UpdateRequirementFieldEventHandler : INotificationHandler<RequirementUpdatedFieldEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<Field> _createEventHelper;

    public UpdateRequirementFieldEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper<Field> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementUpdatedFieldEvent notification, CancellationToken cancellationToken)
    {
        var updatedEvent = await _createEventHelper.CreateEvent(notification.Field);
        await _integrationEventPublisher.PublishAsync(updatedEvent, cancellationToken);
    }
}
