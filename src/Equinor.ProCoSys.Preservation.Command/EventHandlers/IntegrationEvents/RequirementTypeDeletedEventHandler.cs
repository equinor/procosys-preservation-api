using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementTypeDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : INotificationHandler<DeletedEvent<RequirementType>>
{
    public async Task Handle(DeletedEvent<RequirementType> notification, CancellationToken cancellationToken)
    {
        var requirementTypeDeleteEvent = CreateRequirementTypeDeletedEventHelper.CreateEvent(notification.Entity);
        await integrationEventPublisher.PublishAsync(requirementTypeDeleteEvent, cancellationToken);

        foreach (var requirementDefinition in notification.Entity.RequirementDefinitions)
        {
            var requirementDefinitionDeleteEvent = CreateRequirementDefinitionDeletedEventHelper.CreateEvent(requirementDefinition);
            await integrationEventPublisher.PublishAsync(requirementDefinitionDeleteEvent, cancellationToken);
        }
    }
}
