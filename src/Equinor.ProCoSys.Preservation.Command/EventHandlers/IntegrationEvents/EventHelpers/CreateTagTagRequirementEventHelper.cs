using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagTagRequirementEventHelper : ICreateChildEventHelper<Tag, TagRequirement, TagRequirementEvent>
{
    private readonly IRequirementTypeRepository _requirementTypeRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IProjectRepository _projectRepository;

    public CreateTagTagRequirementEventHelper(
        IRequirementTypeRepository requirementTypeRepository,
        IPersonRepository personRepository,
        IProjectRepository projectRepository)
    {
        _requirementTypeRepository = requirementTypeRepository;
        _personRepository = personRepository;
        _projectRepository = projectRepository;
    }

    public async Task<TagRequirementEvent> CreateEvent(Tag parentEntity, TagRequirement entity)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(parentEntity.Guid);
        
        var requirementDefinition = await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(entity.RequirementDefinitionId);
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);

        var modifiedByGuid = await GetModifiedByGuid(entity);

        return new TagRequirementEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            ProjectName = project.Name,
            TagGuid = parentEntity.Guid,
            IntervalWeeks = entity.IntervalWeeks,
            Usage = entity.Usage.ToString(),
            NextDueTimeUtc = entity.NextDueTimeUtc,
            IsVoided = entity.IsVoided,
            IsInUse = entity.IsInUse,
            RequirementDefinitionGuid = requirementDefinition.Guid,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedByGuid,
            ReadyToBePreserved = entity.ReadyToBePreserved
        };
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
