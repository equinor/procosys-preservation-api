using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class PlantEntityCreatedEvent<T> : IPlantEntityEvent<T>, IDomainEvent where T : PlantEntityBase, IHaveGuid
{
    public PlantEntityCreatedEvent(T entity) => Entity = entity;

    public T Entity { get; }    
}
