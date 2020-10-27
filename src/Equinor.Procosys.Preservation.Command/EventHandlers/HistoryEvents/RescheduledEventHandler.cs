using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class RescheduledEventHandler : INotificationHandler<RescheduledEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public RescheduledEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(RescheduledEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.Rescheduled;
            var description = $"{eventType.GetDescription()} - {notification.Weeks}(s) {notification.Direction.ToString().ToLower()}. {notification.Comment}";
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
