using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class RescheduledEventHandler : INotificationHandler<RescheduledEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public RescheduledEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(RescheduledEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.Rescheduled;
            var description = $"{eventType.GetDescription()} - {notification.Weeks} week(s) {notification.Direction.ToString().ToLower()}. {notification.Comment}";
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
