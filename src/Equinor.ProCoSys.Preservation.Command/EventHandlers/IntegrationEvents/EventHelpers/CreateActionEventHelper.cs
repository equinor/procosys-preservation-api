using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Events;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public class CreateActionEventHelper : ICreateEventHelper<Action, ActionEvent>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICreateChildEventHelper<Tag, Action, ActionEvent> _createActionEventHelper;

    public CreateActionEventHelper(
        IProjectRepository projectRepository,
        ICreateChildEventHelper<Tag, Action, ActionEvent> createActionEventHelper)
    {
        _projectRepository = projectRepository;
        _createActionEventHelper = createActionEventHelper;
    }

    public async Task<ActionEvent> CreateEvent(Action entity)
    {
        var tag = await _projectRepository.GetTagByActionGuidAsync(entity.Guid);
        
        return await _createActionEventHelper.CreateEvent(tag, entity);
    }
}
