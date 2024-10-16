using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Converters
{
    public class ProjectTagAddedEventConverter : IDomainToIntegrationEventConverter<ProjectTagAddedEvent>
    {
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IPersonRepository _personRepository;

        public ProjectTagAddedEventConverter(IRequirementTypeRepository requirementTypeRepository, IPersonRepository personRepository)
        {
            _requirementTypeRepository = requirementTypeRepository;
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<IIntegrationEvent>> Convert(ProjectTagAddedEvent domainEvent)
        {
            var events = new List<IIntegrationEvent>();

            foreach (var tagRequirement in domainEvent.Tag.Requirements)
            {
                var projectName = domainEvent.Entity.Name;
                var tagRequirementEvent = await GenerateTagRequirementEvent(tagRequirement, projectName);

                events.Add(tagRequirementEvent);
            }

            return events;
        }

        private async Task<TagRequirementEvent> GenerateTagRequirementEvent(TagRequirement tagRequirement, string projectName)
        {
            var requirementDefinition = await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(tagRequirement.RequirementDefinitionId);
            var createdBy = await _personRepository.GetReadOnlyByIdAsync(tagRequirement.CreatedById);

            var modifiedByGuid = await GetModifiedByGuid(tagRequirement);

            var tagRequirementEvent = new TagRequirementEvent
            {
                ProCoSysGuid = tagRequirement.Guid,
                Plant = tagRequirement.Plant,
                ProjectName = projectName,
                IntervalWeeks = tagRequirement.IntervalWeeks,
                Usage = tagRequirement.Usage.ToString(),
                NextDueTimeUtc = tagRequirement.NextDueTimeUtc,
                IsVoided = tagRequirement.IsVoided,
                IsInUse = tagRequirement.IsInUse,
                RequirementDefinitionGuid = requirementDefinition.Guid,
                CreatedAtUtc = tagRequirement.CreatedAtUtc,
                CreatedByGuid = createdBy.Guid,
                ModifiedAtUtc = tagRequirement.ModifiedAtUtc,
                ModifiedByGuid = modifiedByGuid,
                ReadyToBePreserved = tagRequirement.ReadyToBePreserved
            };
            return tagRequirementEvent;
        }

        private async Task<Guid?> GetModifiedByGuid(TagRequirement tagRequirement)
        {
            if (!tagRequirement.ModifiedById.HasValue)
            {
                return null;
            }

            var modifiedBy = await _personRepository.GetReadOnlyByIdAsync(tagRequirement.ModifiedById.Value);
            if (modifiedBy is null)
            {
                return null;
            }

            return modifiedBy.Guid;
        }
    }
}
