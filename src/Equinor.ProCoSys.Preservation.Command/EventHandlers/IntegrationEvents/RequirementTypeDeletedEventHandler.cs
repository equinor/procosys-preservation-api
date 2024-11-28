using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class RequirementTypeDeletedEventHandler(IPublishDeleteEntityEventHelper<RequirementType> eventPublisher)
    : INotificationHandler<DeletedEvent<RequirementType>>
{
    public async Task Handle(DeletedEvent<RequirementType> notification, CancellationToken cancellationToken) => await eventPublisher.PublishEvent(notification.Entity, cancellationToken);
}
