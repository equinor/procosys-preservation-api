using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
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

            var eventType = EventType.RequirementPreserved;
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
