﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TagVoidedInSourceEventHandler : INotificationHandler<TagVoidedInSourceEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public TagVoidedInSourceEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(TagVoidedInSourceEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.TagVoidedInSource;
            var description = eventType.GetDescription();
            var history = new History(notification.Plant, description, notification.SourceGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
