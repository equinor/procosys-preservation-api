using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[StepEntityName]
public class StepEvent : IStepEventV1
{
    public Guid ProCoSysGuid { get; init; }
    public Guid ModeGuid { get; init; }
    public Guid ResponsibleGuid { get; init; }

    public string Plant { get; init; }

    [JsonIgnore] //ProjectName isnt needed for step but is required for IIntegrationEvent
    public string ProjectName { get; init; } = null;

    public string Title { get; init; }
    public bool IsSupplierStep { get; init; }
    public string AutoTransferMethod { get; init; }
    public int SortKey { get; init; }
    public bool IsVoided { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init; }
}
