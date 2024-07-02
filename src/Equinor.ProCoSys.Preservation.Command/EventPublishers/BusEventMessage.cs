using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.EventPublishers;

public class BusEventMessage : IIntegrationEvent
{
    public Guid Guid { get; set;}
    public Guid ProCoSysGuid => Guid;
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string Behavior { get; set; }
}
