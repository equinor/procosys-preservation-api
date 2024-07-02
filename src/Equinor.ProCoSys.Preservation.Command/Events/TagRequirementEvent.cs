using System;
using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class TagRequirementEvent : ITagRequirementEventV1
{
    public Guid Guid { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public int IntervalWeeks { get; set; }
    public string Usage { get; set; }
    public DateTime? NextDueTimeUtc { get; set; }
    public bool IsVoided { get; set; }
    public bool IsInUse { get; set; }
    public int RequirementDefinitionId { get; set; }
    public Guid RequirementDefinitionGuid { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int CreatedById { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public int? ModifiedById { get; set; }
    public bool ReadyToBePreserved { get; set; }
}
