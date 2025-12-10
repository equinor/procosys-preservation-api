using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class ActionClosedEventHandler : INotificationHandler<ActionClosedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IProjectRepository _projectRepository;

        public ActionClosedEventHandler(IHistoryRepository historyRepository, IProjectRepository projectRepository)
        {
            _historyRepository = historyRepository;
            _projectRepository = projectRepository;
        }

        public async Task Handle(ActionClosedEvent notification, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByActionGuidAsync(notification.Entity.Guid);

            var eventType = EventType.ActionClosed;
            var description = eventType.GetDescription();
            var history = new History(notification.Plant, description, tag.Guid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
        }
    }
}
