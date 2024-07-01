using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MessageContracts;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public interface ICreateEventHelper
{
    Task<IActionEventV1> CreateActionEvent(Action action, Guid tagId);
}
