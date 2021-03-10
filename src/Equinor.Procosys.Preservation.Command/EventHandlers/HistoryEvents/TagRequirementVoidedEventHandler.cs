using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TagRequirementVoidedEventHandler : INotificationHandler<TagRequirementVoidedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public TagRequirementVoidedEventHandler(IHistoryRepository historyRepository, IRequirementTypeRepository requirementTypeRepository)
        {
            _historyRepository = historyRepository;
            _requirementTypeRepository = requirementTypeRepository;
        }

        public Task Handle(TagRequirementVoidedEvent notification, CancellationToken cancellationToken)
        {
            var requirementDefinition =
                _requirementTypeRepository.GetRequirementDefinitionByIdAsync(notification.RequirementDefinitionId);

            var eventType = EventType.RequirementVoided;
            var description = $"{eventType.GetDescription()} - '{requirementDefinition.Result.Title}'";
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType);

            _historyRepository.Add(history);

            return Task.CompletedTask;
        }
    }
}
