using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateActionEventHelper : ICreateEventHelper<Action, ActionEvent>
{
    private readonly IPersonRepository _personRepository;
    private readonly IProjectRepository _projectRepository;

    public CreateActionEventHelper(IPersonRepository personRepository, IProjectRepository projectRepository)
    {
        _personRepository = personRepository;
        _projectRepository = projectRepository;
    }

    public async Task<ActionEvent> CreateEvent(Action entity)
    {
        var tag = await _projectRepository.GetTagByActionGuidAsync(entity.Guid);
        var project = await _projectRepository.GetProjectOnlyByTagGuidAsync(tag.Guid);

        var createdBy = await _personRepository.GetReadOnlyByIdAsync(entity.CreatedById);
        var modifiedBy = entity.ModifiedById.HasValue ? await _personRepository.GetReadOnlyByIdAsync(entity.ModifiedById.Value) : null;

        return new ActionEvent
        {
            ProCoSysGuid = entity.Guid,
            Plant = entity.Plant,
            ProjectName = project.Name,
            TagGuid = tag.Guid,
            Title = entity.Title,
            Description = entity.Description,
            DueDate = entity.DueTimeUtc != null ? DateOnly.FromDateTime(entity.DueTimeUtc.Value) : null,
            Overdue = entity.IsOverDue(),
            Closed = entity.ClosedAtUtc != null ? DateOnly.FromDateTime(entity.ClosedAtUtc.Value) : null,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByGuid = createdBy.Guid,
            ModifiedAtUtc = entity.ModifiedAtUtc,
            ModifiedByGuid = modifiedBy?.Guid,
        };
    }
}
