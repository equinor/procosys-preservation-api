﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class ActionClosedEventHandler : INotificationHandler<ActionClosedEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public ActionClosedEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(ActionClosedEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.ActionClosed;
            var description = eventType.GetDescription();
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}