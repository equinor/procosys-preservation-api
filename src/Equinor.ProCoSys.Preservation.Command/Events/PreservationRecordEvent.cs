using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class PreservationRecordEvent : IPreservationRecordEventV1
{
    public DateTime PreservedAtUtc { get; set; }
    public int PreservedById { get; set; }
    public bool BulkPreserved { get; set; }
}
