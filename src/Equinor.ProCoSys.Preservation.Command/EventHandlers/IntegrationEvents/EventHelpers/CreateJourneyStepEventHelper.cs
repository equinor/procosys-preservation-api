using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateJourneyStepEventHelper(
    IModeRepository modeRepository,
    IPersonRepository personRepository,
    IResponsibleRepository responsibleRepository)
    : ICreateChildEventHelper<Journey, Step, StepEvent>
{
    public async Task<StepEvent> CreateEvent(Journey parentEntity, Step entity)
    {
        var createdBy = await personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        var mode = await modeRepository.GetByIdAsync(entity.ModeId);
        var responsible = await responsibleRepository.GetByIdAsync(entity.ResponsibleId);

        return new StepEvent
        {
            ProCoSysGuid = entity.Guid,
            JourneyGuid = parentEntity.Guid,
            ModeGuid = mode.Guid,
            ResponsibleGuid = responsible.Guid,
            Plant = entity.Plant,
            Title = entity.Title,
            IsSupplierStep = entity.IsSupplierStep,
            AutoTransferMethod = entity.AutoTransferMethod.ToString(),
            SortKey = entity.SortKey,
            IsVoided = entity.IsVoided,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid
        };
    }
}
