using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.EventHandlers.HistoryEvents
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
