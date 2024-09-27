using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public interface IEntityDeleteEvent<TEntity> : IDeleteEventV1
    where TEntity : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
{
    
}
