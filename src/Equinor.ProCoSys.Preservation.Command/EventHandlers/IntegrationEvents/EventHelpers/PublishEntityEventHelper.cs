using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class PublishEntityEventHelper<TEntity, TEvent> : IPublishEntityEventHelper<TEntity>
    where TEntity : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    where TEvent : class, IIntegrationEvent
{
    private readonly ICreateEventHelper<TEntity, TEvent> _createEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public PublishEntityEventHelper(ICreateEventHelper<TEntity, TEvent> createEventHelper, IIntegrationEventPublisher integrationEventPublisher)
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
