﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

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
            var history = new History(notification.Plant, description, notification.SourceGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
