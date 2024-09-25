using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class IntegrationEventHandler<TEvent, TEntity> : INotificationHandler<TEvent> 
    where TEvent: IPlantEntityEvent<TEntity>, INotification
    where TEntity : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<TEntity> _createEventHelper;

    public IntegrationEventHandler(IIntegrationEventPublisher integrationEventPublisher,
        ICreateEventHelper<TEntity> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(TEvent notification, CancellationToken cancellationToken)
    {
        var actionEvent = await _createEventHelper.CreateEvent(notification.Entity);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }
}
