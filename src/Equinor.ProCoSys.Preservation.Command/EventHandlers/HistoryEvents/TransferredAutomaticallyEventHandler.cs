﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TransferredAutomaticallyEventHandler : INotificationHandler<TransferredAutomaticallyEvent>
    {
        private readonly IHistoryRepository _historyRepository;

        public TransferredAutomaticallyEventHandler(IHistoryRepository historyRepository) => _historyRepository = historyRepository;

        public Task Handle(TransferredAutomaticallyEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.TransferredAutomatically;
            var description = $"{eventType.GetDescription()} - From '{notification.FromStep}' to '{notification.ToStep}'. Transfer method was {notification.AutoTransferMethod.CovertToString()}";
            var history = new History(notification.Plant, description, notification.SourceGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
