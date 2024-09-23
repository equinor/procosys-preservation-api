using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class AddResponsibleEventHandler : INotificationHandler<ResponsibleAddedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper<Responsible> _createEventHelper;

    public AddResponsibleEventHandler(IIntegrationEventPublisher integrationEventPublisher,
        ICreateEventHelper<Responsible> createEventHelper)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
    }

    public async Task Handle(ResponsibleAddedEvent notification, CancellationToken cancellationToken)
    {
        var actionEvent = await _createEventHelper.CreateEvent(notification.Responsible);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }
}
