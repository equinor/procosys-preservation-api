using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Audit;

namespace Equinor.ProCoSys.Preservation.Command.EventHandlers.IntegrationEvents.EventHelpers;

public interface IPublishDeleteEntityEventHelper<in TEntity> 
    where TEntity : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
{
    Task PublishEvent(TEntity entity, CancellationToken cancellationToken);
}
