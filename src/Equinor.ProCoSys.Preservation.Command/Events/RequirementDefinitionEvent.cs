using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class RequirementDefinitionEvent : IRequirementDefinitionEventV1
{
    public Guid Guid { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string Title { get; set; }
    public bool IsVoided { get; set; }
    public int DefaultIntervalWeeks { get; set; }
    public string Usage { get; set; }
    public int SortKey { get; set; }
    public bool NeedsUserInput { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int CreatedById { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public int? ModifiedById { get; set; }
}
