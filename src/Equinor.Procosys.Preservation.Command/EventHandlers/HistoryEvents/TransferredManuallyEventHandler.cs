using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TransferredManuallyEventHandler : INotificationHandler<TransferredManuallyEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public TransferredManuallyEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(TransferredManuallyEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.TransferredManually;
            var description = $"{eventType.GetDescription()} - From '{notification.FromStep}' to '{notification.ToStep}'";
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
