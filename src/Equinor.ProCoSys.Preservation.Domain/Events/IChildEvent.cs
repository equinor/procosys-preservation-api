using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public interface IChildEvent<out TEntity, out TChild>
    : IPlantEntityEvent<TEntity>, IDomainEvent
    where TEntity : PlantEntityBase, IHaveGuid
    where TChild : PlantEntityBase, IHaveGuid
{
    public TEntity Entity { get; }
    public TChild ChildEntity { get; }
}
