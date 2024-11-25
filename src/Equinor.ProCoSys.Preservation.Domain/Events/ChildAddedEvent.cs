using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ChildAddedEvent<TEntity, TChild>(TEntity entity, TChild childEntity)
    : IChildEvent<TEntity, TChild>
    where TEntity : PlantEntityBase, IHaveGuid
    where TChild : PlantEntityBase, IHaveGuid
{
    public TEntity Entity { get; } = entity;
    public TChild ChildEntity { get; } = childEntity;
}
