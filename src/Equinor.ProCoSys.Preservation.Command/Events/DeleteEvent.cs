using System;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.Audit;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public abstract class DeleteEvent<TEntity> : IEntityDeleteEvent<TEntity>
    where TEntity : PlantEntityBase, ICreationAuditable, IModificationAuditable, IHaveGuid
{
    protected DeleteEvent(Guid guid, string plant, string projectName)
    {
        ProCoSysGuid = guid;
        Plant = plant;
        ProjectName = projectName;
    }

    public Guid ProCoSysGuid { get; }
    public string Plant { get; }
    public string ProjectName { get; }
    public string Behavior => "delete";
}
