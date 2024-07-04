using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventPublishers;

public class BusEventMessage : IIntegrationEvent
{
    public Guid Guid { get; init;}
    public Guid ProCoSysGuid => Guid;
    public string Plant { get; init; }
    public string ProjectName { get; init; }
    public string Behavior { get; set; }
}
