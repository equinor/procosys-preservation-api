using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementDefinitionDeletedEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : INotificationHandler<DeletedEvent<RequirementDefinition>>
{
    public async Task Handle(DeletedEvent<RequirementDefinition> notification, CancellationToken cancellationToken)
    {
        var integrationEvents = CreateRequirementDefinitionDeletedEventHelper.CreateEvents(notification.Entity);
        foreach (var integrationEvent in integrationEvents)
        {
            await integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
        }
    }
}
