using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateRequirementDefinitionEventHelper : ICreateEventHelper<RequirementDefinition, RequirementDefinitionEvent>
{
    private readonly IPersonRepository _personRepository;
    private readonly IRequirementTypeRepository _requirementTypeRepository;

    public CreateRequirementDefinitionEventHelper(IPersonRepository personRepository, IRequirementTypeRepository requirementTypeRepository)
    {
        _personRepository = personRepository;
        _requirementTypeRepository = requirementTypeRepository;
    }

    public async Task<RequirementDefinitionEvent> CreateEvent(RequirementDefinition entity)
    {
        var requirementType = await _requirementTypeRepository.GetRequirementTypeByRequirementDefinitionGuidAsync(entity.Guid);
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new RequirementDefinitionEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            Title = entity.Title,
            IsVoided = entity.IsVoided,
            DefaultIntervalWeeks = entity.DefaultIntervalWeeks,
            Usage = entity.Usage.ToString(),
            SortKey = entity.SortKey,
            NeedsUserInput = entity.NeedsUserInput,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            RequirementTypeGuid = requirementType.Guid
        };
    }
}
