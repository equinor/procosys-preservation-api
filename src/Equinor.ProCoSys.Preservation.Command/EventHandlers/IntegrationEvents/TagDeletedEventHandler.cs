using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers.EventCollections;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class TagDeletedEventHandler(
    ICreateTagDeleteEventHelper createEventHelper,
    IIntegrationEventPublisher integrationEventPublisher)
    : INotificationHandler<DeletedEvent<Tag>>
{
    public async Task Handle(DeletedEvent<Tag> notification, CancellationToken cancellationToken)
    {
        var integrationEvents = await createEventHelper.CreateEvents(notification.Entity);
        await integrationEventPublisher.PublishAsync(integrationEvents.TagDeleteEvent, cancellationToken);

        foreach (var actionDeleteEvent in integrationEvents.ActionDeleteEvents)
        {
            await integrationEventPublisher.PublishAsync(actionDeleteEvent, cancellationToken);
        }
        
        foreach (var tagRequirementDeleteEvent in integrationEvents.TagRequirementDeleteEvents)
        {
            await HandleTagRequirementDeleteEvent(tagRequirementDeleteEvent, cancellationToken);
        }
    }

    private async Task HandleTagRequirementDeleteEvent(
        EventCollectionDeleteTagRequirement eventCollectionDeleteTagRequirement,
        CancellationToken cancellationToken)
    {
        await integrationEventPublisher.PublishAsync(eventCollectionDeleteTagRequirement.TagRequirementDeleteEvent, cancellationToken);

        foreach (var preservationPeriodDeleteEvent in eventCollectionDeleteTagRequirement.PreservationPeriodDeleteEvents)
        {
            await integrationEventPublisher.PublishAsync(preservationPeriodDeleteEvent, cancellationToken);
        }
    }
}
