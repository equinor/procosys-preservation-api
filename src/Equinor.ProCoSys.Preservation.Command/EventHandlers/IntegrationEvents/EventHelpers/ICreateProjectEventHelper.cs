using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public interface ICreateProjectEventHelper<in TEntity, TEvent> 
    where TEntity : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    where TEvent : class, IIntegrationEvent
{
    Task<TEvent> CreateEvent(TEntity entity, string projectName);
}
