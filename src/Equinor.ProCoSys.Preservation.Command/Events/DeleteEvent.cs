using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class DeleteEvent : IDeleteEventV1
{
    public DeleteEvent(Guid guid, string plant, string projectName)
    {
        Guid = guid;
        Plant = plant;
        ProjectName = projectName;
    }

    public Guid Guid { get; }
    public Guid ProCoSysGuid => Guid;
    public string Plant { get; }
    public string ProjectName { get; }
    public string Behavior { get; } = "delete";
}
