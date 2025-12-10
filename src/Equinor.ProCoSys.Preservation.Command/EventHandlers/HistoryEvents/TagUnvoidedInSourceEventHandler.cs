using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TagUnvoidedInSourceEventHandler : INotificationHandler<TagUnvoidedInSourceEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public TagUnvoidedInSourceEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(TagUnvoidedInSourceEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.TagUnvoidedInSource;
            var description = eventType.GetDescription();
            var history = new History(notification.Plant, description, notification.SourceGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
