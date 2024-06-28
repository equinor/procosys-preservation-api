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
    private readonly IProjectRepository _projectRepository;

    public ActionAddedEventHandler(IIntegrationEventPublisher integrationEventPublisher,
        ICreateEventHelper createEventHelper,
        IReadOnlyContext context,
        IProjectRepository projectRepository)
    {
        _integrationEventPublisher = integrationEventPublisher;
        _createEventHelper = createEventHelper;
        _context = context;
        _projectRepository = projectRepository;
    }

    public async Task Handle(ActionAddedEvent notification, CancellationToken cancellationToken)
    {
        var tagObject = await _projectRepository.GetTagOnlyByGuidAsync(notification.SourceGuid);

        var actionObject = await (from action in _context.QuerySet<Action>()
                                where EF.Property<int>(action, "TagId") == tagObject.Id
                                where action.Guid == notification.ActionGuid
                                select action).SingleOrDefaultAsync(cancellationToken);

        var actionEvent = await _createEventHelper.CreateActionEvent(actionObject, tagObject);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);
    }
}
