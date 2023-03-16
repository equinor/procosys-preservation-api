using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class UndoPreservationStartedEventHandler : INotificationHandler<UndoPreservationStartedEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public UndoPreservationStartedEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(UndoPreservationStartedEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.UndoPreservationStarted;
            var description = eventType.GetDescription();
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
