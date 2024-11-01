using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class ChildEntityAddedEventHandler<TParent, TChild, TEvent>  : INotificationHandler<ChildEntityAddedEvent<TParent, TChild>>
    where TParent : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    where TChild : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    where TEvent : class, IIntegrationEvent
{
    private readonly ICreateChildEventHelper<TParent, TChild, TEvent> _createTagEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public ChildEntityAddedEventHandler(
        ICreateChildEventHelper<TParent, TChild, TEvent> createTagEventHelper,
        IIntegrationEventPublisher integrationEventPublisher)
    {
        _createTagEventHelper = createTagEventHelper;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task Handle(ChildEntityAddedEvent<TParent, TChild> notification, CancellationToken cancellationToken)
    {
        var integrationEvent = await _createTagEventHelper.CreateEvent(notification.Entity, notification.ChildEntity);
        await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
