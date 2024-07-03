using System;
using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class PreservationPeriodsEvent : IPreservationPeriodEventV1
{
    public Guid TagRequirementGuid { get; set; }
    public string Status { get; set; }
    public DateTime DueTimeUtc { get; set; }
    public string Comment { get; set; }
    public Guid PreservationRecordGuid { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int CreatedById { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public int? ModifiedById { get; set; }
    public Guid Guid { get; set; }
    public Guid ProCoSysGuid => Guid;
    public string Plant { get; set; }
    public string ProjectName { get; set; } = "";
    public dynamic PreservationRecord { get; set; }
}
