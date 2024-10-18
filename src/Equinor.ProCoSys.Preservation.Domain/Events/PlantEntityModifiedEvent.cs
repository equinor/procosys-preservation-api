using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class PlantEntityModifiedEvent<T> : IPlantEntityEvent<T>, IDomainEvent where T : PlantEntityBase, IHaveGuid
{
    public PlantEntityModifiedEvent(T entity) => Entity = entity;

    public T Entity { get; }    
}
