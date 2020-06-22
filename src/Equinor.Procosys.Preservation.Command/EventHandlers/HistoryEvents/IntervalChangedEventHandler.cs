using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class IntervalChangedEventHandler : INotificationHandler<IntervalChangedEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public IntervalChangedEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(IntervalChangedEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.IntervalChanged;
            var description = eventType.GetDescription() + " - From: " + notification.FromInterval + " To: " + notification.ToInterval;
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
