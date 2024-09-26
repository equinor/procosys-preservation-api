using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.Command.Events.EntityNames;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

[JourneyEntityName]
public class JourneyEvent : IJourneyEventV1
{
    public Guid ProCoSysGuid { get; init; }
    public string Plant { get; init; }

    [JsonIgnore]
    public string ProjectName { get; } = null;
    
    public string Title { get; init; }
    public bool IsVoided { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init; }
}
