using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents;

public interface ICreateEventHelper
{
    Task<IActionEventV1> CreateActionEvent(Action action, Tag tag);
}
