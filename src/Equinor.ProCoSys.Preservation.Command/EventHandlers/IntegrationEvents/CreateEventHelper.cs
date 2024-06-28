using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public class CreateEventHelper : ICreateEventHelper
{
    private readonly IProjectRepository _projectRepository;

    public CreateEventHelper(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IActionEventV1> CreateActionEvent(Action action, Tag tag)
    {
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
}
