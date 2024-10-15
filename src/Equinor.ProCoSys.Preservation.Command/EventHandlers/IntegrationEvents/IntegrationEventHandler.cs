using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class IntegrationEventHandler<TNotificationEvent, TEntity>
    : INotificationHandler<TNotificationEvent> 
    where TNotificationEvent : class, IPlantEntityEvent<TEntity>, IPostSaveDomainEvent
    where TEntity : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
{
    private readonly IPublishEntityEventHelper<TEntity> _eventPublisher;

    public IntegrationEventHandler(IPublishEntityEventHelper<TEntity> eventPublisher) => _eventPublisher = eventPublisher;

    public async Task Handle(TNotificationEvent notification, CancellationToken cancellationToken) => await _eventPublisher.PublishEvent(notification.Entity, cancellationToken);
}
