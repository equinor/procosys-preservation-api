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

public class ChildEventHandler<TParent, TChild, TEvent>(
    ICreateChildEventHelper<TParent, TChild, TEvent> createTagEventHelper,
    IIntegrationEventPublisher integrationEventPublisher)
    : INotificationHandler<IChildEvent<TParent, TChild>>
    where TParent : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    where TChild : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    where TEvent : class, IIntegrationEvent
{
    public async Task Handle(IChildEvent<TParent, TChild> notification, CancellationToken cancellationToken)
    {
        var integrationEvent = await createTagEventHelper.CreateEvent(notification.Entity, notification.ChildEntity);
        await integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
