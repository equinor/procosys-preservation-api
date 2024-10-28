using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class PlantEntityDeletedEvent<T> : IPlantEntityEvent<T>, IDomainEvent where T : PlantEntityBase, IHaveGuid
{
    public PlantEntityDeletedEvent(T entity) => Entity = entity;

    public T Entity { get; }
}
