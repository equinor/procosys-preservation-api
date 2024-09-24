using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TagRequirementAddedEventHandler : INotificationHandler<TagRequirementAddedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IProjectRepository _projectRepository;

        public TagRequirementAddedEventHandler(IHistoryRepository historyRepository, IRequirementTypeRepository requirementTypeRepository, IProjectRepository projectRepository)
        {
            _historyRepository = historyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _projectRepository = projectRepository;
        }

        public async Task Handle(TagRequirementAddedEvent notification, CancellationToken cancellationToken)
        {
            var requirementDefinition =
                _requirementTypeRepository.GetRequirementDefinitionByIdAsync(notification.TagRequirement.RequirementDefinitionId);
            var tag = await _projectRepository.GetTagByTagRequirementGuidAsync(notification.TagRequirement.Guid);

            var eventType = EventType.RequirementAdded;
            var description = $"{eventType.GetDescription()} - '{requirementDefinition.Result.Title}'";
            var history = new History(notification.Plant, description, tag.Guid, ObjectType.Tag, eventType);

            _historyRepository.Add(history);
        }
    }
}
