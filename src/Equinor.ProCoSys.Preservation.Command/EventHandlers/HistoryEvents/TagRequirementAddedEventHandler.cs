using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TagRequirementAddedEventHandler : INotificationHandler<TagRequirementAddedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public TagRequirementAddedEventHandler(IHistoryRepository historyRepository, IRequirementTypeRepository requirementTypeRepository)
        {
            _historyRepository = historyRepository;
            _requirementTypeRepository = requirementTypeRepository;
        }

        public Task Handle(TagRequirementAddedEvent notification, CancellationToken cancellationToken)
        {
            var requirementDefinition =
                _requirementTypeRepository.GetRequirementDefinitionByIdAsync(notification.RequirementDefinitionId);

            var eventType = EventType.RequirementAdded;
            var description = $"{eventType.GetDescription()} - '{requirementDefinition.Result.Title}'";
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);

            _historyRepository.Add(history);

            return Task.CompletedTask;
        }
    }
}
