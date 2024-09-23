using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class AddTagRequirementEventHandler  : INotificationHandler<TagRequirementAddedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<TagRequirement> _createEventHelper;

    public AddTagRequirementEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper<TagRequirement> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(TagRequirementAddedEvent notification, CancellationToken cancellationToken)
    {
        var actionEvent = await _createEventHelper.CreateEvent(notification.TagRequirement);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }
}
