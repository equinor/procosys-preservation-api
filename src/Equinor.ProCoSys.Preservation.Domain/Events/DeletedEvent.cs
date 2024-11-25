using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class DeletedEvent<T> : IPlantEntityEvent<T>, IDomainEvent where T : PlantEntityBase, IHaveGuid
{
    public DeletedEvent(T entity) => Entity = entity;

    public T Entity { get; }
}
