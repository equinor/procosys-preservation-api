using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.Audit;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class PublishDeleteEntityEventHelper<TEntity, TEvent> : IPublishDeleteEntityEventHelper<TEntity>
    where TEntity : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    where TEvent : class, IEntityDeleteEvent<TEntity>
{
    private readonly ICreateEventHelper<TEntity, TEvent> _createEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public PublishDeleteEntityEventHelper(ICreateEventHelper<TEntity, TEvent> createEventHelper, IIntegrationEventPublisher integrationEventPublisher)
    {
        _createEventHelper = createEventHelper;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task PublishEvent(TEntity entity, CancellationToken cancellationToken)
    {
        var integrationEvent = await _createEventHelper.CreateEvent(entity);
        await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
