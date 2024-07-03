using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class RequirementDefinitionEvent : IRequirementDefinitionEventV1
{
    public Guid Guid { get; set; }
    public Guid ProCoSysGuid => Guid;
    public string Plant { get; set; }

    [JsonIgnore] //ProjectName isnt needed for RequirementDefinition but is required for IIntegrationEvent
    public string ProjectName { get; set; } = null;

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
