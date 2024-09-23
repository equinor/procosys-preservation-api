using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.Context;

public interface ITagProjectId
{
    Task<int> Retrieve(Tag tag);
}
