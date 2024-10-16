using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters
{
    public class ProjectTagAddedEventConverter : IDomainToIntegrationEventConverter<ProjectTagAddedEvent>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;

        public ProjectTagAddedEventConverter(IRequirementTypeRepository requirementTypeRepository) => _requirementTypeRepository = requirementTypeRepository;

        public async Task<IEnumerable<IIntegrationEvent>> Convert(ProjectTagAddedEvent domainEvent)
        {
            var events = new List<IIntegrationEvent>();

            foreach (var tagRequirement in domainEvent.Tag.Requirements)
            {
                var requirementDefinition = await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(tagRequirement.RequirementDefinitionId);

                var tagRequirementEvent = new TagRequirementEvent
                {
                    ProCoSysGuid = tagRequirement.Guid,
                    Plant = tagRequirement.Plant,
                    ProjectName = domainEvent.Entity.Name,
                    IntervalWeeks = tagRequirement.IntervalWeeks,
                    Usage = tagRequirement.Usage.ToString(),
                    NextDueTimeUtc = tagRequirement.NextDueTimeUtc,
                    IsVoided = tagRequirement.IsVoided,
                    IsInUse = tagRequirement.IsInUse,
                    RequirementDefinitionGuid = requirementDefinition.Guid,
                    CreatedAtUtc = tagRequirement.CreatedAtUtc,
                    ModifiedAtUtc = tagRequirement.ModifiedAtUtc,
                    ReadyToBePreserved = tagRequirement.ReadyToBePreserved
                };

                events.Add(tagRequirementEvent);
            }

            return events;
        }
    }
}
