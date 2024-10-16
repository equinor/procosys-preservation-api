﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class ActionAddedEventHandler : INotificationHandler<ActionAddedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IProjectRepository _projectRepository;

        public ActionAddedEventHandler(IHistoryRepository historyRepository, IProjectRepository projectRepository)
        {
            _historyRepository = historyRepository;
            _projectRepository = projectRepository;
        }

        public Task Handle(ActionAddedEvent notification, CancellationToken cancellationToken)
        {
            var eventType = EventType.ActionAdded;
            var description = $"{eventType.GetDescription()} - '{notification.Entity.Title}'";
            var history = new History(notification.Plant, description, notification.SourceGuid, ObjectType.Tag, eventType);
            _historyRepository.Add(history);
            return Task.CompletedTask;
        }
    }
}
