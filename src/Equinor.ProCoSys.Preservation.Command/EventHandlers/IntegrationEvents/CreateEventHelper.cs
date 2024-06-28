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
    private readonly IPlantProvider _plantProvider;

    public CreateEventHelper(IPlantProvider plantProvider)
    {
        _plantProvider = plantProvider;
    }

    public async Task<IActionEventV1> CreateActionEvent(Action action, Tag tag)
    {
        return new ActionEvent(
            action.Guid,
            action.Plant,
            "",
            tag.TagNo,
            action.Title,
            action.Description,
            action.DueTimeUtc != null ? DateOnly.FromDateTime(action.DueTimeUtc.GetValueOrDefault(DateTime.Now)) : null,
            action.IsOverDue(),
            action.ClosedAtUtc != null ? DateOnly.FromDateTime(action.ClosedAtUtc.GetValueOrDefault(DateTime.Now)) : null
            );
    }
}
