using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Context;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateTagEventHelper  : ICreateEventHelper<Tag>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITagProjectId _tagProjectId;

    public CreateTagEventHelper(
        IJourneyRepository journeyRepository,
        IPersonRepository personRepository,
        IProjectRepository projectRepository,
        ITagProjectId tagProjectId)
    {
        _journeyRepository = journeyRepository;
        _personRepository = personRepository;
        _projectRepository = projectRepository;
        _tagProjectId = tagProjectId;
    }

    public async Task<IIntegrationEvent> CreateEvent(Tag entity)
    {
        var projectId = await _tagProjectId.Retrieve(entity);
        var project = await _projectRepository.GetByIdAsync(projectId);
        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;
        var step = await _journeyRepository.GetStepByStepIdAsync(entity.StepId);

        return new TagEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            ProjectName = project?.Name,
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
            CommPkgGuid = entity.CommPkgProCoSysGuid,
            McPkgGuid = entity.McPkgProCoSysGuid,
            IsVoided = entity.IsVoided,
            IsVoidedInSource = entity.IsVoidedInSource
        };
    }
}
