using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementTypeAddedEventHandler : INotificationHandler<RequirementTypeAddedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper _createEventHelper;

    public RequirementTypeAddedEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(RequirementTypeAddedEvent notification, CancellationToken cancellationToken)
    {
        var requirementTypeEvent = await _createEventHelper.CreateRequirementTypeEvent(notification.RequirementType);
        await _integrationEventPublisher.PublishAsync(requirementTypeEvent, cancellationToken);
    }
}
