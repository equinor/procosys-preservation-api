using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class RequirementTypeEvent : IRequirementTypeEventV1
{
    public Guid Guid { get; set; }
    public Guid ProCoSysGuid => Guid;
    public string Plant { get; set; }

    [JsonIgnore] //ProjectName isnt needed for RequirementType but is required for IIntegrationEvent
    public string ProjectName { get; set; } = null;

    public string Code { get; set; }
    public string Title { get; set; }
    public bool IsVoided { get; set; }
    public int SortKey { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int CreatedById { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public int? ModifiedById { get; set; }
}
