using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Microsoft.EntityFrameworkCore;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class CreateEventHelper : ICreateEventHelper
{
    private readonly IProjectRepository _projectRepository;
    private readonly IReadOnlyContext _context;
    private readonly IRequirementTypeRepository _requirementTypeRepository;
    private readonly IPersonRepository _personRepository;

    public CreateEventHelper(IProjectRepository projectRepository, IReadOnlyContext context, IRequirementTypeRepository requirementTypeRepository, IPersonRepository personRepository)
    {
        _projectRepository = projectRepository;
        _context = context;
        _requirementTypeRepository = requirementTypeRepository;
        _personRepository = personRepository;
    }

    public async Task<IActionEventV1> CreateActionEvent(Action action)
    {
        var tagId = await _context.QuerySet<Action>().Where(s => s.Guid == action.Guid)
            .Select(s => EF.Property<int>(s, "TagId")).SingleAsync();
        var tag = await _projectRepository.GetTagOnlyByTagIdAsync(tagId);
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tag.Guid);

        return new ActionEvent(
            action.Guid,
            action.Plant,
            project.Name,
            tag.Guid,
            action.Title,
            action.Description,
            action.DueTimeUtc != null ? DateOnly.FromDateTime(action.DueTimeUtc.Value) : null,
            action.IsOverDue(),
            action.ClosedAtUtc != null ? DateOnly.FromDateTime(action.ClosedAtUtc.Value) : null
            );
    }

    public async Task<ITagRequirementEventV1> CreateRequirementEvent(TagRequirement tagRequirement)
    {
        var tagId = await _context.QuerySet<TagRequirement>().Where(s => s.Guid == tagRequirement.Guid)
            .Select(s => EF.Property<int>(s, "TagId")).SingleAsync();
        var tag = await _projectRepository.GetTagOnlyByTagIdAsync(tagId);
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tag.Guid);
        var requirementDefinitions =
            await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(tagRequirement.RequirementDefinitionId);

        var createdBy = await _personRepository.GetByIdAsync(tagRequirement.CreatedById);
        var modifiedBy = tagRequirement.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(tagRequirement.ModifiedById.Value) : null;

        return new TagRequirementEvent
        {
            Guid = tagRequirement.Guid,
            Plant = tagRequirement.Plant,
            ProjectName = project.Name,
            IntervalWeeks = tagRequirement.IntervalWeeks,
            Usage = tagRequirement.Usage.ToString(),
            NextDueTimeUtc = tagRequirement.NextDueTimeUtc,
            IsVoided = tagRequirement.IsVoided,
            IsInUse = tagRequirement.IsInUse,
            RequirementDefinitionGuid = requirementDefinitions.Guid,
            CreatedAtUtc = tagRequirement.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = tagRequirement.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            ReadyToBePreserved = tagRequirement.ReadyToBePreserved
        };
    }

    public async Task<IPreservationPeriodEventV1> CreatePreservationPeriodEvent(PreservationPeriod preservationPeriod)
    {
        var preservationRecord = preservationPeriod.PreservationRecord;
        var tagRequirement = await _context.QuerySet<TagRequirement>().SingleAsync(rd => rd.Id == preservationPeriod.TagRequirementId);

        var createdBy = await _personRepository.GetByIdAsync(preservationPeriod.CreatedById);
        var modifiedBy = preservationPeriod.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(preservationPeriod.ModifiedById.Value) : null;
        var preservedBy = preservationPeriod.PreservationRecord != null ? await _personRepository.GetByIdAsync(preservationPeriod.PreservationRecord.PreservedById) : null;

        return new PreservationPeriodsEvent
        {
            Status = preservationPeriod.Status.ToString(),
            DueTimeUtc = preservationPeriod.DueTimeUtc,
            Comment = preservationPeriod.Comment,
            PreservationRecordGuid = preservationPeriod.PreservationRecord?.Guid,
            CreatedAtUtc = preservationPeriod.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = preservationPeriod.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            TagRequirementGuid = tagRequirement.Guid,
            Guid = preservationPeriod.Guid,
            Plant = preservationPeriod.Plant,
            PreservedAtUtc = preservationRecord?.PreservedAtUtc,
            PreservedByGuid = preservedBy?.Guid,
            BulkPreserved = preservationRecord?.BulkPreserved
        };
    }

    public async Task<IFieldEventV1> CreateFieldEvent(Field field)
    {
        var definitionId = await _context.QuerySet<Field>().Where(s => s.Guid == field.Guid)
            .Select(s => EF.Property<int>(s, "RequirementDefinitionId")).SingleAsync();
        var requirementDefinition = await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(definitionId);

        var createdBy = await _personRepository.GetByIdAsync(field.CreatedById);
        var modifiedBy = field.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(field.ModifiedById.Value) : null;

        return new FieldEvent
        {
            RequirementDefinitionGuid = requirementDefinition.Guid,
            Label = field.Label,
            Unit = field.Unit,
            SortKey = field.SortKey,
            FieldType = field.FieldType.ToString(),
            CreatedAtUtc = field.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = field.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            Guid = field.Guid,
            Plant = field.Plant
        };
    }
    public async Task<IRequirementDefinitionEventV1> CreateRequirementDefinitionEvent(RequirementDefinition requirementDefinition){
        var typeId = await _context.QuerySet<RequirementDefinition>().Where(s => s.Guid == requirementDefinition.Guid)
            .Select(s => EF.Property<int>(s, "RequirementTypeId")).SingleAsync();
        var requirementType = await _context.QuerySet<RequirementType>().SingleAsync(rd => rd.Id == typeId);

        var createdBy = await _personRepository.GetByIdAsync(requirementDefinition.CreatedById);
        var modifiedBy = requirementDefinition.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(requirementDefinition.ModifiedById.Value) : null;

        return new RequirementDefinitionEvent
        {
            Guid = requirementDefinition.Guid,
            Plant = requirementDefinition.Plant,
            Title = requirementDefinition.Title,
            IsVoided = requirementDefinition.IsVoided,
            DefaultIntervalWeeks = requirementDefinition.DefaultIntervalWeeks,
            Usage = requirementDefinition.Usage.ToString(),
            SortKey = requirementDefinition.SortKey,
            NeedsUserInput = requirementDefinition.NeedsUserInput,
            CreatedAtUtc = requirementDefinition.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = requirementDefinition.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            RequirementTypeGuid = requirementType.Guid
        };
    }

    public async Task<IRequirementTypeEventV1> CreateRequirementTypeEvent(RequirementType requirementType)
    {
        var createdBy = await _personRepository.GetByIdAsync(requirementType.CreatedById);
        var modifiedBy = requirementType.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(requirementType.ModifiedById.Value) : null;

        return new RequirementTypeEvent
        {
            Guid = requirementType.Guid,
            Plant = requirementType.Plant,
            Code = requirementType.Code,
            Title = requirementType.Title,
            IsVoided = requirementType.IsVoided,
            SortKey = requirementType.SortKey,
            CreatedAtUtc = requirementType.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = requirementType.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid
        };
    }

    public async Task<ITagEventV1> CreateTagEvent(Tag tag)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tag.Guid);
        var createdBy = await _personRepository.GetByIdAsync(tag.CreatedById);
        var modifiedBy = tag.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(tag.ModifiedById.Value) : null;

        return new TagEvent
        {
            Guid = tag.Guid,
            Plant = tag.Plant,
            ProjectName = project.Name,
            Description = tag.Description,
            Remark = tag.Remark,
            NextDueTimeUtc = tag.NextDueTimeUtc,
            StepId = tag.StepId,
            DisciplineCode = tag.DisciplineCode,
            AreaCode = tag.AreaCode,
            TagFunctionCode = tag.TagFunctionCode,
            PurchaseOrderNo = tag.PurchaseOrderNo,
            TagType = tag.TagType.ToString(),
            StorageArea = tag.StorageArea,
            AreaDescription = tag.AreaDescription,
            DisciplineDescription = tag.DisciplineDescription,
            CreatedAtUtc = tag.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = tag.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            Status = tag.Status.ToString(),
            CommPkgGuid = tag.CommPkgProCoSysGuid,
            McPkgGuid = tag.McPkgProCoSysGuid,
            IsVoided = tag.IsVoided,
            IsVoidedInSource = tag.IsVoidedInSource
        };
    }
}
