using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TagRequirementPreservedEventHandler : INotificationHandler<TagRequirementPreservedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public TagRequirementPreservedEventHandler(IHistoryRepository historyRepository, IRequirementTypeRepository requirementTypeRepository)
        {
            _historyRepository = historyRepository;
            _requirementTypeRepository = requirementTypeRepository;
        }

        public Task Handle(TagRequirementPreservedEvent notification, CancellationToken cancellationToken)
        {
            var requirementDefinition =
                _requirementTypeRepository.GetRequirementDefinitionByIdAsync(notification.RequirementDefinitionId);

            var eventType = EventType.TagRequirementPreserved;
            var description = $"{eventType.GetDescription()} - '{requirementDefinition.Result.Title}'";
            var history = new History(notification.Plant, description, notification.ObjectGuid, ObjectType.Tag, eventType)
            {
                DueInWeeks = notification.DueInWeeks,
                PreservationRecordGuid = notification.PreservationRecordGuid
            };

            _historyRepository.Add(history);

            return Task.CompletedTask;
        }
    }
}
