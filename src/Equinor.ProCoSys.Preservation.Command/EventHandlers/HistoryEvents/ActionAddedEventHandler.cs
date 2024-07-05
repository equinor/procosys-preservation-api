using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class ActionAddedEventHandler : INotificationHandler<ActionAddedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IReadOnlyContext _context;

        public ActionAddedEventHandler(IHistoryRepository historyRepository, IProjectRepository projectRepository, IReadOnlyContext context)
        {
            _historyRepository = historyRepository;
            _projectRepository = projectRepository;
            _context = context;
        }

        public async Task Handle(ActionAddedEvent notification, CancellationToken cancellationToken)
        {
            var tagId = await _context.QuerySet<Action>().Where(s => s.Guid == notification.Action.Guid)
                .Select(s => EF.Property<int>(s, "TagId")).SingleAsync(cancellationToken: cancellationToken);
            var tag = await _projectRepository.GetTagOnlyByTagIdAsync(tagId);

            var eventType = EventType.ActionAdded;
            var description = $"{eventType.GetDescription()} - '{notification.Action.Title}'";
            var history = new History(notification.Plant, description, tag.Guid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
        }
    }
}
