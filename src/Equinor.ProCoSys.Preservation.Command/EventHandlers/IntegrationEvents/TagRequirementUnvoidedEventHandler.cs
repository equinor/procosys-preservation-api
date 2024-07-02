using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class TagRequirementUnvoidedEventHandler  : INotificationHandler<TagRequirementUnvoidedEvent>
{
    private readonly ICreateEventHelper _createEventHelper;
    public TagRequirementUnvoidedEventHandler(ICreateEventHelper createEventHelper) => _createEventHelper = createEventHelper;

    public async Task Handle(TagRequirementUnvoidedEvent notification, CancellationToken cancellationToken) => await _createEventHelper.SendTagRequirementEvents(notification.TagRequirement, notification.SourceGuid, cancellationToken);
}
