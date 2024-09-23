using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateModeEventHelper : ICreateEventHelper<Mode>
{
    private readonly IPersonRepository _personRepository;

    public CreateModeEventHelper(IPersonRepository personRepository) => _personRepository = personRepository;

    public async Task<IIntegrationEvent> CreateEvent(Mode entity)
    {
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new ModeEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            Title = entity.Title,
            IsVoided = entity.IsVoided,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            ForSupplier = entity.ForSupplier
        };
    }
}
