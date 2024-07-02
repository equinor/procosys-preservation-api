using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class PreservationRecordEvent : IPreservationRecordEventV1
{
    public DateTime PreservedAtUtc { get; set; }
    public int PreservedById { get; set; }
    public bool BulkPreserved { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int CreatedById { get; set; }
    public Guid Guid { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string Plant { get; set; }
    public string ProjectName { get; set; }
}
