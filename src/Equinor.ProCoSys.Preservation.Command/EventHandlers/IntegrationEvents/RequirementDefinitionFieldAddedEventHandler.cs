using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementDefinitionFieldAddedEventHandler : INotificationHandler<EntityAddedChildEntityEvent<RequirementDefinition, Field>>
{
    private readonly ICreateChildEventHelper<RequirementDefinition, Field, FieldEvent> _createTagEventHelper;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public RequirementDefinitionFieldAddedEventHandler(ICreateChildEventHelper<RequirementDefinition, Field, FieldEvent> createTagEventHelper, IIntegrationEventPublisher integrationEventPublisher)
    {
        _createTagEventHelper = createTagEventHelper;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task Handle(EntityAddedChildEntityEvent<RequirementDefinition, Field> notification, CancellationToken cancellationToken)
    {
        var integrationEvent = await _createTagEventHelper.CreateEvent(notification.Entity, notification.ChildEntity);
        await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
