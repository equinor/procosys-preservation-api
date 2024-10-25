using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateRequirementDefinitionFieldEventHelper : ICreateChildEventHelper<RequirementDefinition, Field, FieldEvent>
{
    private readonly IPersonRepository _personRepository;

    public CreateRequirementDefinitionFieldEventHelper(IPersonRepository personRepository) => _personRepository = personRepository;

    public async Task<FieldEvent> CreateEvent(RequirementDefinition parentEntity, Field entity)
    {
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new FieldEvent
        {
            RequirementDefinitionGuid = parentEntity.Guid,
            Label = entity.Label,
            Unit = entity.Unit,
            SortKey = entity.SortKey,
            FieldType = entity.FieldType.ToString(),
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant
        };
    }
}
