using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class PreservationCompletedEventHandler : INotificationHandler<PreservationCompletedEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public PreservationCompletedEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(PreservationCompletedEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.PreservationCompleted;
            var description = eventType.GetDescription();
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
