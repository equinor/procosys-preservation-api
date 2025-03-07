using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class TagRequirementDeletedEventHandler(
    IIntegrationEventPublisher integrationEventPublisher,
    IProjectRepository projectRepository)
    : INotificationHandler<TagRequirementDeletedEvent>
{
    public async Task Handle(TagRequirementDeletedEvent notification, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetProjectOnlyByTagGuidAsync(notification.SourceGuid);

        var deleteEvents = CreateTagRequirementDeleteEventHelper.CreateEvents(notification.Entity, project);
        await integrationEventPublisher.PublishAsync(deleteEvents.TagRequirementDeleteEvent, cancellationToken);

        foreach (var preservationPeriodDeleteEvents in deleteEvents.PreservationPeriodDeleteEvents)
        {
            await integrationEventPublisher.PublishAsync(preservationPeriodDeleteEvents, cancellationToken);
        }
    }
}
