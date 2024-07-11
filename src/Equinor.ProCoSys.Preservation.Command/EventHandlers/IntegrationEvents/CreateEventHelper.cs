using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
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
    private readonly IModeRepository _modeRepository;
    private readonly IResponsibleRepository _responsibleRepository;
    private readonly IJourneyRepository _journeyRepository;

    public CreateEventHelper(IProjectRepository projectRepository, IReadOnlyContext context, IRequirementTypeRepository requirementTypeRepository, IPersonRepository personRepository, IModeRepository modeRepository, IResponsibleRepository responsibleRepository, IJourneyRepository journeyRepository)
    {
        _projectRepository = projectRepository;
        _context = context;
        _requirementTypeRepository = requirementTypeRepository;
        _personRepository = personRepository;
        _modeRepository = modeRepository;
        _responsibleRepository = responsibleRepository;
        _journeyRepository = journeyRepository;
    }

    public async Task<IActionEventV1> CreateActionEvent(Action action)
    {
        var tag = await _projectRepository.GetTagByActionGuidAsync(action.Guid);
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tag.Guid);

        var createdBy = await _personRepository.GetByIdAsync(action.CreatedById);
        var modifiedBy = action.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(action.ModifiedById.Value) : null;

        return new ActionEvent
        {
            ProCoSysGuid = action.Guid,
            Plant = action.Plant,
            ProjectName = project.Name,
            TagGuid = tag.Guid,
            Title = action.Title,
            Description = action.Description,
            DueDate = action.DueTimeUtc != null ? DateOnly.FromDateTime(action.DueTimeUtc.Value) : null,
            Overdue = action.IsOverDue(),
            Closed = action.ClosedAtUtc != null ? DateOnly.FromDateTime(action.ClosedAtUtc.Value) : null,
            CreatedAtUtc = action.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = action.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
        };
    }

    public async Task<ITagRequirementEventV1> CreateRequirementEvent(TagRequirement tagRequirement)
    {
        var tag = await _projectRepository.GetTagByTagRequirementGuidAsync(tagRequirement.Guid);
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tag.Guid);
        var requirementDefinitions =
            await _requirementTypeRepository.GetRequirementDefinitionByIdAsync(tagRequirement.RequirementDefinitionId);

        var createdBy = await _personRepository.GetByIdAsync(tagRequirement.CreatedById);
        var modifiedBy = tagRequirement.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(tagRequirement.ModifiedById.Value) : null;

        return new TagRequirementEvent
        {
            ProCoSysGuid = tagRequirement.Guid,
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
            CreatedAtUtc = preservationPeriod.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = preservationPeriod.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            TagRequirementGuid = tagRequirement.Guid,
            ProCoSysGuid = preservationPeriod.Guid,
            Plant = preservationPeriod.Plant,
            PreservedAtUtc = preservationRecord?.PreservedAtUtc,
            PreservedByGuid = preservedBy?.Guid,
            BulkPreserved = preservationRecord?.BulkPreserved
        };
    }

    public async Task<IFieldEventV1> CreateFieldEvent(Field field)
    {
        var requirementDefinition = await _requirementTypeRepository.GetRequirementDefinitionByFieldGuidAsync(field.Guid);
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
            ProCoSysGuid = field.Guid,
            Plant = field.Plant
        };
    }
    public async Task<IRequirementDefinitionEventV1> CreateRequirementDefinitionEvent(RequirementDefinition requirementDefinition){
        var requirementType = await _requirementTypeRepository.GetRequirementTypeByRequirementDefinitionGuidAsync(requirementDefinition.Guid);
        var createdBy = await _personRepository.GetByIdAsync(requirementDefinition.CreatedById);
        var modifiedBy = requirementDefinition.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(requirementDefinition.ModifiedById.Value) : null;

        return new RequirementDefinitionEvent
        {
            ProCoSysGuid = requirementDefinition.Guid,
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
            ProCoSysGuid = requirementType.Guid,
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
        var step = await _journeyRepository.GetStepByStepIdAsync(tag.StepId);

        return new TagEvent
        {
            ProCoSysGuid = tag.Guid,
            Plant = tag.Plant,
            ProjectName = project.Name,
            Description = tag.Description,
            Remark = tag.Remark,
            NextDueTimeUtc = tag.NextDueTimeUtc,
            StepGuid = step.Guid,
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

    public async Task<IModeEventV1> CreateModeEvent(Mode mode)
    {
        var createdBy = await _personRepository.GetByIdAsync(mode.CreatedById);
        var modifiedBy = mode.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(mode.ModifiedById.Value) : null;

        return new ModeEvent
        {
            ProCoSysGuid = mode.Guid,
            Plant = mode.Plant,
            Title = mode.Title,
            IsVoided = mode.IsVoided,
            CreatedAtUtc = mode.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = mode.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
            ForSupplier = mode.ForSupplier
        };
    }

    public async Task<IJourneyEventV1> CreateJourneyEvent(Journey journey)
    {
        var createdBy = await _personRepository.GetByIdAsync(journey.CreatedById);
        var modifiedBy = journey.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(journey.ModifiedById.Value) : null;

        return new JourneyEvent
        {
            ProCoSysGuid = journey.Guid,
            Plant = journey.Plant,
            Title = journey.Title,
            IsVoided = journey.IsVoided,
            CreatedAtUtc = journey.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = journey.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid
        };
    }

    public async Task<IResponsibleEventV1> CreateResponsibleEvent(Responsible responsible)
    {
        var createdBy = await _personRepository.GetByIdAsync(responsible.CreatedById);
        var modifiedBy = responsible.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(responsible.ModifiedById.Value) : null;

        return new ResponsibleEvent
        {
            ProCoSysGuid = responsible.Guid,
            Plant = responsible.Plant,
            Code = responsible.Code,
            Description = responsible.Description,
            IsVoided = responsible.IsVoided,
            CreatedAtUtc = responsible.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = responsible.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid
        };
    }

    public async Task<IStepEventV1> CreateStepEvent(Step step)
    {
        var createdBy = await _personRepository.GetByIdAsync(step.CreatedById);
        var modifiedBy = step.ModifiedById.HasValue ? await _personRepository.GetByIdAsync(step.ModifiedById.Value) : null;

        var mode = await _modeRepository.GetByIdAsync(step.ModeId);
        var responsible = await _responsibleRepository.GetByIdAsync(step.ResponsibleId);

        return new StepEvent
        {
            ProCoSysGuid = step.Guid,
            ModeGuid = mode.Guid,
            ResponsibleGuid = responsible.Guid,
            Plant = step.Plant,
            Title = step.Title,
            IsSupplierStep = step.IsSupplierStep,
            AutoTransferMethod = step.AutoTransferMethod.ToString(),
            SortKey = step.SortKey,
            IsVoided = step.IsVoided,
            CreatedAtUtc = step.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = step.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid
        };
    }
}
