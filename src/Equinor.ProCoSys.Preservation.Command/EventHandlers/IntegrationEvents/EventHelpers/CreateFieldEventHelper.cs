using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateFieldEventHelper : ICreateEventHelper<Field, FieldEvent>
{
    private readonly IPersonRepository _personRepository;
    private readonly IRequirementTypeRepository _requirementTypeRepository;

    public CreateFieldEventHelper(IPersonRepository personRepository, IRequirementTypeRepository requirementTypeRepository)
    {
        _personRepository = personRepository;
        _requirementTypeRepository = requirementTypeRepository;
    }

    public async Task<FieldEvent> CreateEvent(Field entity)
    {
        var requirementDefinition = await _requirementTypeRepository.GetRequirementDefinitionByFieldGuidAsync(entity.Guid);
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new FieldEvent
        {
            RequirementDefinitionGuid = requirementDefinition.Guid,
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
