using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Context;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreatePreservationPeriodEventHelper : ICreateEventHelper<PreservationPeriod>
{
    private readonly IPersonRepository _personRepository;
    private readonly IPreservationPeriodTagRequirement _preservationPeriodTagRequirement;

    public CreatePreservationPeriodEventHelper(IPersonRepository personRepository, IPreservationPeriodTagRequirement preservationPeriodTagRequirement)
    {
        _personRepository = personRepository;
        _preservationPeriodTagRequirement = preservationPeriodTagRequirement;
    }

    public async Task<IIntegrationEvent> CreateEvent(PreservationPeriod entity)
    {
        var preservationRecord = entity.PreservationRecord;
        var tagRequirement = await _preservationPeriodTagRequirement.Retrieve(entity);

        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;
        var preservedBy = entity.PreservationRecord != null ? await _personRepository.GetReadOnlyByIdAsync(entity.PreservationRecord.PreservedById) : null;

        return new PreservationPeriodsEvent
        {
            Status = entity.Status.ToString(),
            DueTimeUtc = entity.DueTimeUtc,
            Comment = entity.Comment,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            TagRequirementGuid = tagRequirement.Guid,
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            PreservedAtUtc = preservationRecord?.PreservedAtUtc,
            PreservedByGuid = preservedBy?.Guid,
            BulkPreserved = preservationRecord?.BulkPreserved
        };
    }
}
