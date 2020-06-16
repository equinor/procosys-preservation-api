using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.EventHandlers
{
    public class HistoryEventHandler : INotificationHandler<HistoryEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public HistoryEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(HistoryEvent notification, CancellationToken cancellationToken)
        {
            var description = notification.EventType.GetDescription();
            var history = new History(notification.Plant, description, notification.ObjectGuid,
                notification.ObjectType, notification.EventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
