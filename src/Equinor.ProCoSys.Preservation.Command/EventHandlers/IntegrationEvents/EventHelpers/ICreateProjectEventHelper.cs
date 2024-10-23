using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public interface ICreateChildEventHelper<in TParent, in TChild, TEvent> 
    where TParent : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    where TChild : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
    where TEvent : class, IIntegrationEvent
{
    Task<TEvent> CreateEvent(TParent parentEntity, TChild childEntity);
}
