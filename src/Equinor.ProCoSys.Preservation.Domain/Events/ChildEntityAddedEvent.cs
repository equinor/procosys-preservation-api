using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Domain.Events;

public class ChildEntityAddedEvent<TEntity, TChild> : IPlantEntityEvent<TEntity>, IDomainEvent 
    where TEntity : PlantEntityBase, IHaveGuid 
    where TChild : PlantEntityBase, IHaveGuid
{
    public ChildEntityAddedEvent(TEntity entity, TChild childEntity)
    {
        Entity = entity;
        ChildEntity = childEntity;
    }

    public TEntity Entity { get; }
    public TChild ChildEntity { get; }
}
