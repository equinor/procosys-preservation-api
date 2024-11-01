using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagRequirementPreservationPeriodEventHelper : ICreateChildEventHelper<TagRequirement, PreservationPeriod, PreservationPeriodsEvent>
{
    private readonly IPersonRepository _personRepository;

    public CreateTagRequirementPreservationPeriodEventHelper(IPersonRepository personRepository) => _personRepository = personRepository;

    public async Task<PreservationPeriodsEvent> CreateEvent(TagRequirement parentEntity, PreservationPeriod entity)
    {
        var preservationRecord = entity.PreservationRecord;

        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;
        var preservedBy = entity.PreservationRecord != null ? await _personRepository.GetReadOnlyByIdAsync(entity.PreservationRecord.PreservedById) : null;

        // TODO test
        return new PreservationPeriodsEvent
        {
            Status = entity.Status.ToString(),
            DueTimeUtc = entity.DueTimeUtc,
            Comment = entity.Comment,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            TagRequirementGuid = parentEntity.Guid,
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            PreservedAtUtc = preservationRecord?.PreservedAtUtc,
            PreservedByGuid = preservedBy?.Guid,
            BulkPreserved = preservationRecord?.BulkPreserved
        };
    }
}
