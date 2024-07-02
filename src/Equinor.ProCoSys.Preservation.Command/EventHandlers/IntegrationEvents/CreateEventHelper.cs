using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.EventPublishers;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class CreateEventHelper : ICreateEventHelper
{
    private readonly IProjectRepository _projectRepository;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public CreateEventHelper(IProjectRepository projectRepository, IIntegrationEventPublisher integrationEventPublisher)
    {
        _projectRepository = projectRepository;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task<IActionEventV1> CreateActionEvent(Action action, Guid tagGuid)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tagGuid);

        return new ActionEvent(
            action.Guid,
            action.Plant,
            project.Name,
            tagGuid,
            action.Title,
            action.Description,
            action.DueTimeUtc != null ? DateOnly.FromDateTime(action.DueTimeUtc.Value) : null,
            action.IsOverDue(),
            action.ClosedAtUtc != null ? DateOnly.FromDateTime(action.ClosedAtUtc.Value) : null
            );
    }

    public async Task SendTagRequirementEvents(TagRequirement tagRequirement, Guid tagGuid, CancellationToken cancellationToken)
    {
        var actionEvent = await CreateRequirementEvent(tagRequirement, tagGuid);
        await _integrationEventPublisher.PublishAsync(actionEvent, cancellationToken);

        foreach (var period in tagRequirement.PreservationPeriods)
        {
            var preservationPeriodEvent = await CreatePreservationPeriodEvent(period, tagRequirement.Guid, tagGuid);
            await _integrationEventPublisher.PublishAsync(preservationPeriodEvent, cancellationToken);

            var preservationRecordEvent = await CreatePreservationRecordEvent(period.PreservationRecord, tagRequirement.Guid, period.Guid, tagGuid);
            await _integrationEventPublisher.PublishAsync(preservationRecordEvent, cancellationToken);
        }
    }

    public async Task<ITagRequirementEventV1> CreateRequirementEvent(TagRequirement tagRequirement, Guid tagGuid)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tagGuid);

        return new TagRequirementEvent
        {
            Guid = tagRequirement.Guid,
            ProCoSysGuid = tagRequirement.Guid,
            Plant = tagRequirement.Plant,
            ProjectName = project.Name,
            IntervalWeeks = tagRequirement.IntervalWeeks,
            Usage = tagRequirement.Usage.ToString(),
            NextDueTimeUtc = tagRequirement.NextDueTimeUtc,
            IsVoided = tagRequirement.IsVoided,
            IsInUse = tagRequirement.IsInUse,
            RequirementDefinitionId = tagRequirement.RequirementDefinitionId,
            RequirementDefinitionGuid = tagRequirement.RequirementDefinitionGuid,
            CreatedAtUtc = tagRequirement.CreatedAtUtc,
            CreatedById = tagRequirement.CreatedById,
            ModifiedAtUtc = tagRequirement.ModifiedAtUtc,
            ModifiedById = tagRequirement.ModifiedById,
            ReadyToBePreserved = tagRequirement.ReadyToBePreserved
        };
    }

    public async Task<IPreservationPeriodEventV1> CreatePreservationPeriodEvent(PreservationPeriod preservationPeriod, Guid tagRequirementGuid, Guid tagGuid)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tagGuid);

        return new PreservationPeriodsEvent
        {
            TagRequirementGuid = tagRequirementGuid,
            Status = preservationPeriod.Status.ToString(),
            DueTimeUtc = preservationPeriod.DueTimeUtc,
            Comment = preservationPeriod.Comment,
            PreservationRecordGuid = preservationPeriod.PreservationRecord.Guid,
            CreatedAtUtc = preservationPeriod.CreatedAtUtc,
            CreatedById = preservationPeriod.CreatedById,
            ModifiedAtUtc = preservationPeriod.ModifiedAtUtc,
            ModifiedById = preservationPeriod.ModifiedById,
            TagRequirementId = preservationPeriod.TagRequirementId,
            Guid = preservationPeriod.Guid,
            ProCoSysGuid = preservationPeriod.Guid,
            Plant = preservationPeriod.Plant,
            ProjectName = project.Name
        };
    }

    public async Task<IPreservationRecordEventV1> CreatePreservationRecordEvent(PreservationRecord preservationRecord, Guid tagRequirementGuid,
        Guid preservationPeriodGuid, Guid tagGuid)
    {
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tagGuid);

        return new PreservationRecordEvent
        {
            PreservedAtUtc = preservationRecord.PreservedAtUtc,
            PreservedById = preservationRecord.PreservedById,
            BulkPreserved = preservationRecord.BulkPreserved,
            CreatedAtUtc = preservationRecord.CreatedAtUtc,
            CreatedById = preservationRecord.CreatedById,
            Guid = preservationRecord.Guid,
            ProCoSysGuid = preservationRecord.Guid,
            Plant = preservationRecord.Plant,
            ProjectName = project.Name
        };
    }

    public async Task<IFieldEventV1> CreateFieldEvent(Field field, Guid sourceGuid) =>
        new FieldEvent
        {
            RequirementDefinitionGuid = sourceGuid,
            FieldId = field.Id,
            Label = field.Label,
            Unit = field.Unit,
            SortKey = field.SortKey,
            FieldType = field.FieldType.ToString(),
            CreatedAtUtc = field.CreatedAtUtc,
            CreatedById = field.CreatedById,
            ModifiedAtUtc = field.ModifiedAtUtc,
            ModifiedById = field.ModifiedById,
            Guid = field.Guid,
            ProCoSysGuid = field.Guid,
            Plant = field.Plant
        };

    public async Task<IRequirementDefinitionEventV1> CreateRequirementDefinitionEvent(RequirementDefinition requirementDefinition) =>
        new RequirementDefinitionEvent
        {
            Guid = requirementDefinition.Guid,
            ProCoSysGuid = requirementDefinition.Guid,
            Plant = requirementDefinition.Plant,
            ProjectName = "",
            Title = requirementDefinition.Title,
            IsVoided = requirementDefinition.IsVoided,
            DefaultIntervalWeeks = requirementDefinition.DefaultIntervalWeeks,
            Usage = requirementDefinition.Usage.ToString(),
            SortKey = requirementDefinition.SortKey,
            NeedsUserInput = requirementDefinition.NeedsUserInput,
            CreatedAtUtc = requirementDefinition.CreatedAtUtc,
            CreatedById = requirementDefinition.CreatedById,
            ModifiedAtUtc = requirementDefinition.ModifiedAtUtc,
            ModifiedById = requirementDefinition.ModifiedById
        };
}
