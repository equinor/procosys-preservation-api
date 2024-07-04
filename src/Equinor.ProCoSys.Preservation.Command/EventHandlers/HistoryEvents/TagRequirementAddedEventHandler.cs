using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.HistoryEvents
{
    public class TagRequirementAddedEventHandler : INotificationHandler<TagRequirementAddedEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly PreservationContext _context;

        public TagRequirementAddedEventHandler(IHistoryRepository historyRepository, IRequirementTypeRepository requirementTypeRepository, IProjectRepository projectRepository, PreservationContext context)
        {
            _historyRepository = historyRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _projectRepository = projectRepository;
            _context = context;
        }

        public async Task Handle(TagRequirementAddedEvent notification, CancellationToken cancellationToken)
        {
            var requirementDefinition =
                _requirementTypeRepository.GetRequirementDefinitionByIdAsync(notification.TagRequirement.RequirementDefinitionId);

            var tagId = await _context.Actions.Where(s => s.Guid == notification.TagRequirement.Guid)
                .Select(s => EF.Property<int>(s, "TagId")).SingleAsync(cancellationToken: cancellationToken);
            var tag = await _projectRepository.GetTagOnlyByTagIdAsync(tagId);

            var eventType = EventType.RequirementAdded;
            var description = $"{eventType.GetDescription()} - '{requirementDefinition.Result.Title}'";
            var history = new History(notification.Plant, description, tag.Guid, ObjectType.Tag, eventType);

            _historyRepository.Add(history);
        }
    }
}
