using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagEventHelper  : ICreateProjectEventHelper<Tag, TagEvent>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IPersonRepository _personRepository;

    public CreateTagEventHelper(IJourneyRepository journeyRepository, IPersonRepository personRepository)
    {
        _journeyRepository = journeyRepository;
        _personRepository = personRepository;
    }

    public async Task<TagEvent> CreateEvent(Tag entity, string projectName)
    {
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = await GetModifiedBy(entity);
        var step = await _journeyRepository.GetStepByStepIdAsync(entity.StepId);

        return new TagEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            ProjectName = projectName,
            Description = entity.Description,
            Remark = entity.Remark,
            NextDueTimeUtc = entity.NextDueTimeUtc,
            StepGuid = step.Guid,
            DisciplineCode = entity.DisciplineCode,
            AreaCode = entity.AreaCode,
            TagFunctionCode = entity.TagFunctionCode,
            PurchaseOrderNo = entity.PurchaseOrderNo,
            TagType = entity.TagType.ToString(),
            StorageArea = entity.StorageArea,
            AreaDescription = entity.AreaDescription,
            DisciplineDescription = entity.DisciplineDescription,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            Status = entity.Status.ToString(),
            // CommPkgGuid = entity.CommPkgProCoSysGuid,
            // McPkgGuid = entity.McPkgProCoSysGuid,
            // IsVoided = entity.IsVoided,
            // IsVoidedInSource = entity.IsVoidedInSource
        };
    }

    private async Task<Person> GetModifiedBy(Tag entity)
    {
        if (!entity.ModifiedById.HasValue)
        {
            return null;
        }

        return await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value);
    }
}
