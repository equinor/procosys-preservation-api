using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class CreatedEvent<T>(T entity) : IPlantEntityEvent<T>, IDomainEvent
    where T : PlantEntityBase, IHaveGuid
{
    public T Entity { get; } = entity;
}
