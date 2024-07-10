using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class ModeEvent : IModeEventV1
{
    public Guid Guid { get; init; }
    public string Plant { get; init; }

    [JsonIgnore]
    public string ProjectName { get; } = null;

    public string Title { get; init; }
    public bool IsVoided { get; init; }
    public bool ForSupplier { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init; }
}
