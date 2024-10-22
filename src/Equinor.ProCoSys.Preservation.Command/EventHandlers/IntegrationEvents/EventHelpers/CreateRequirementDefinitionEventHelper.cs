﻿using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateRequirementDefinitionEventHelper : ICreateChildEventHelper<RequirementType, RequirementDefinition, RequirementDefinitionEvent>
{
    private readonly IPersonRepository _personRepository;

    public CreateRequirementDefinitionEventHelper(IPersonRepository personRepository) => _personRepository = personRepository;

    public async Task<RequirementDefinitionEvent> CreateEvent(RequirementType parentEntity, RequirementDefinition entity)
    {
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        // TODO test this
        return new RequirementDefinitionEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            Title = entity.Title,
            IsVoided = entity.IsVoided,
            DefaultIntervalWeeks = entity.DefaultIntervalWeeks,
            Usage = nameof(entity.Usage),
            SortKey = entity.SortKey,
            NeedsUserInput = entity.NeedsUserInput,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            RequirementTypeGuid = parentEntity.Guid
        };
    }
}
