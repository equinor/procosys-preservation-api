using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public abstract class DeleteEvent : IDeleteEventV1
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
