using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagRequirementEventEventHelper : ICreateEventHelper<TagRequirement, TagRequirementEvent>
{
    private readonly IPersonRepository _personRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IRequirementTypeRepository _requirementTypeRepository;

    public CreateTagRequirementEventEventHelper(
        IPersonRepository personRepository,
        IProjectRepository projectRepository,
        IRequirementTypeRepository requirementTypeRepository)
    {
        _personRepository = personRepository;
        _projectRepository = projectRepository;
        _requirementTypeRepository = requirementTypeRepository;
    }

    public async Task<TagRequirementEvent> CreateEvent(TagRequirement entity)
    {
        var tag = await _projectRepository.GetTagByTagRequirementGuidAsync(entity.Guid);
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tag.Guid);
        var requirementDefinitions = await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(entity.RequirementDefinitionId);

        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new TagRequirementEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            ProjectName = project.Name,
            IntervalWeeks = entity.IntervalWeeks,
            Usage = entity.Usage.ToString(),
            NextDueTimeUtc = entity.NextDueTimeUtc,
            IsVoided = entity.IsVoided,
            IsInUse = entity.IsInUse,
            RequirementDefinitionGuid = requirementDefinitions.Guid,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            ReadyToBePreserved = entity.ReadyToBePreserved
        };
    }
}
