using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class DeleteTagRequirementEventHandler(
    IIntegrationEventPublisher integrationEventPublisher,
    IProjectRepository projectRepository)
    : INotificationHandler<TagRequirementDeletedEvent>
{
    public async Task Handle(TagRequirementDeletedEvent notification, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetProjectOnlyByTagGuidAsync(notification.SourceGuid);

        var deleteEvent = CreateTagRequirementDeleteEventHelper.CreateEvent(notification.Entity, project);
        await integrationEventPublisher.PublishAsync(deleteEvent, cancellationToken);
    }
}
