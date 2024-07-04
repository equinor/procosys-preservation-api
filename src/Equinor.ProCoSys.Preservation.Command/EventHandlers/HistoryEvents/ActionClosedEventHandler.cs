using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class ActionClosedEventHandler : INotificationHandler<ActionClosedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly PreservationContext _context;

        public ActionClosedEventHandler(IHistoryRepository historyRepository, IProjectRepository projectRepository, PreservationContext context)
        {
            _historyRepository = historyRepository;
            _projectRepository = projectRepository;
            _context = context;
        }

        public async Task Handle(ActionClosedEvent notification, CancellationToken cancellationToken)
        {
            var tagId = await _context.Actions.Where(s => s.Guid == notification.Action.Guid)
                .Select(s => EF.Property<int>(s, "TagId")).SingleAsync(cancellationToken: cancellationToken);
            var tag = await _projectRepository.GetTagOnlyByTagIdAsync(tagId);

            var eventType = EventType.ActionClosed;
            var description = eventType.GetDescription();
            var history = new History(notification.Plant, description, tag.Guid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
        }
    }
}
