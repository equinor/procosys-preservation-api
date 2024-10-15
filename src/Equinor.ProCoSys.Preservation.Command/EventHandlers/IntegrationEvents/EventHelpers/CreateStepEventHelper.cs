using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateStepEventHelper : ICreateEventHelper<Step, StepEvent>
{
    private readonly IModeRepository _modeRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IResponsibleRepository _responsibleRepository;

    public CreateStepEventHelper(IModeRepository modeRepository, IPersonRepository personRepository, IResponsibleRepository responsibleRepository)
    {
        _modeRepository = modeRepository;
        _personRepository = personRepository;
        _responsibleRepository = responsibleRepository;
    }

    public async Task<StepEvent> CreateEvent(Step entity)
    {
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        var mode = await _modeRepository.GetByIdAsync(entity.ModeId);
        var responsible = await _responsibleRepository.GetByIdAsync(entity.ResponsibleId);

        return new StepEvent
        {
            ProCoSysGuid = entity.Guid,
            ModeGuid = mode.Guid,
            ResponsibleGuid = responsible.Guid,
            Plant = entity.Plant,
            Title = entity.Title,
            IsSupplierStep = entity.IsSupplierStep,
            AutoTransferMethod = nameof(entity.AutoTransferMethod),
            SortKey = entity.SortKey,
            IsVoided = entity.IsVoided,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid
        };
    }
}
