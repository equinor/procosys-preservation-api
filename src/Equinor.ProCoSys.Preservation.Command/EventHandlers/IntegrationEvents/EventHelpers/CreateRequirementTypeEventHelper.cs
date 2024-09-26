using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateRequirementTypeEventHelper : ICreateEventHelper<RequirementType, RequirementTypeEvent>
{
    private readonly IPersonRepository _personRepository;

    public CreateRequirementTypeEventHelper(IPersonRepository personRepository) => _personRepository = personRepository;

    public async Task<RequirementTypeEvent> CreateEvent(RequirementType entity)
    {
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new RequirementTypeEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            Code = entity.Code,
            Title = entity.Title,
            IsVoided = entity.IsVoided,
            SortKey = entity.SortKey,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid
        };
    }
}
