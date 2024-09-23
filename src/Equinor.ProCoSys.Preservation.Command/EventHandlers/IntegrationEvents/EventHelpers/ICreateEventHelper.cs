using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public interface ICreateEventHelper<in T> where T : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
{
    Task<IIntegrationEvent> CreateEvent(T entity);
}
