using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ModifiedEvent<T> : IPlantEntityEvent<T>, IDomainEvent where T : PlantEntityBase, IHaveGuid
{
    public ModifiedEvent(T entity) => Entity = entity;

    public T Entity { get; }    
}
