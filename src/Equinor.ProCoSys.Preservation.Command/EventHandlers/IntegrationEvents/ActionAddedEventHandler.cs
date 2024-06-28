using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class ActionAddedEventHandler : INotificationHandler<ActionAddedEvent>
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICreateEventHelper _createEventHelper;
    private readonly IReadOnlyContext _context;

    public ActionAddedEventHandler(IIntegrationEventPublisher integrationEventPublisher, ICreateEventHelper createEventHelper, IReadOnlyContext context)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
        _context = context;
    }

    public async Task Handle(ActionAddedEvent notification, CancellationToken cancellationToken)
    {
        var tagObject = await (from tag in _context.QuerySet<Tag>()
                                        where tag.Guid == notification.SourceGuid
                                        select tag).SingleOrDefaultAsync(cancellationToken);

        var actionObject = await (from action in _context.QuerySet<Action>()
                                where EF.Property<int>(action, "TagId") == tagObject.Id
                                where action.Title == notification.Title
                                select action).SingleOrDefaultAsync(cancellationToken);

        var actionEvent = await _createEventHelper.CreateActionEvent(actionObject, tagObject);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }
}
