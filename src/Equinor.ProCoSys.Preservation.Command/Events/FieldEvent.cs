using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[FieldEntityName]
public class FieldEvent : IFieldEventV1
{
    public Guid ProCoSysGuid { get; init; }
    public Guid RequirementDefinitionGuid { get; init; }

    public string Plant { get; init; }

    [JsonIgnore]
    public string ProjectName { get; init; }

    public string Label { get; init; }
    public string Unit { get; init; }
    public int SortKey { get; init; }
    public string FieldType { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init; }
}
