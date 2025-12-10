using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class ActionAddedEventHandler : INotificationHandler<ChildAddedEvent<Tag, Action>>
    {
        private readonly IHistoryRepository _historyRepository;

        public ActionAddedEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(ChildAddedEvent<Tag, Action> notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.ActionAdded;
            var description = $"{eventType.GetDescription()} - '{notification.ChildEntity.Title}'";
            var history = new History(notification.ChildEntity.Plant, description, notification.Entity.Guid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
