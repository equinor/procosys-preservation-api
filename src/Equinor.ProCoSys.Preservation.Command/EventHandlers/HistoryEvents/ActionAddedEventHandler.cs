using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class ActionAddedEventHandler : INotificationHandler<TagActionAddedEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public ActionAddedEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(TagActionAddedEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.ActionAdded;
            var description = $"{eventType.GetDescription()} - '{notification.Action.Title}'";
            var history = new History(notification.Action.Plant, description, notification.Entity.Guid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
