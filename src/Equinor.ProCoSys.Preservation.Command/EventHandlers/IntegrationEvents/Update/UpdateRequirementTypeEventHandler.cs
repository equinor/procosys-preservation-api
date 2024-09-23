using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class UpdateRequirementTypeEventHandler : INotificationHandler<RequirementTypeUpdatedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<RequirementType> _createEventHelper;

    public UpdateRequirementTypeEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper<RequirementType> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementTypeUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var requirementTypeEvent = await _createEventHelper.CreateEvent(notification.RequirementType);
        await _integrationEventPublisher.PublishAsync(requirementTypeEvent, cancellationToken);
    }
}
