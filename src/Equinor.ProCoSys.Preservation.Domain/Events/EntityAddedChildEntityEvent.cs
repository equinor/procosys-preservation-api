using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class EntityAddedChildEntityEvent<TEntity, TChild> : IPlantEntityEvent<TEntity>, IDomainEvent 
    where TEntity : PlantEntityBase, IHaveGuid 
    where TChild : PlantEntityBase, IHaveGuid
{
    public EntityAddedChildEntityEvent(TEntity entity, TChild childEntity)
    {
        Entity = entity;
        ChildEntity = childEntity;
    }

    public TEntity Entity { get; }
    public TChild ChildEntity { get; }
}
