using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateResponsibleEventHelper : ICreateEventHelper<Responsible, ResponsibleEvent>
{
    private readonly IPersonRepository _personRepository;

    public CreateResponsibleEventHelper(IPersonRepository personRepository) => _personRepository = personRepository;

    public async Task<ResponsibleEvent> CreateEvent(Responsible entity)
    {
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new ResponsibleEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            Code = entity.Code,
            IsVoided = entity.IsVoided,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid
        };
    }
}
