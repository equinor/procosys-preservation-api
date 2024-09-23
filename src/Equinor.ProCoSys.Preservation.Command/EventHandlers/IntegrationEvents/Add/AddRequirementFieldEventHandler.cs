using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class AddRequirementFieldEventHandler : INotificationHandler<RequirementAddedFieldEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<Field> _createEventHelper;

    public AddRequirementFieldEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper<Field> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementAddedFieldEvent notification, CancellationToken cancellationToken)
    {
        var addedEvent = await _createEventHelper.CreateEvent(notification.Field);
        await _integrationEventPublisher.PublishAsync(addedEvent, cancellationToken);
    }
}
