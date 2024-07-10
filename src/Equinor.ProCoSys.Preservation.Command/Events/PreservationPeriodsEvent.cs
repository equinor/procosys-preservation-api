using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class PreservationPeriodsEvent : IPreservationPeriodEventV1
{
    public Guid Guid { get; init; }
    public string Plant { get; init; }
    [JsonIgnore] //ProjectName isnt needed for RequirementDefinition but is required for IIntegrationEvent
    public string ProjectName { get; init; } = null;

    public Guid TagRequirementGuid { get; init; }
    public string Status { get; init; }
    public DateTime DueTimeUtc { get; init; }
    public string Comment { get; init; }
    public Guid? PreservationRecordGuid { get; init; }
    public DateTime? PreservedAtUtc { get; init; }
    public Guid? PreservedByGuid { get; init;}
    public bool? BulkPreserved { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init; }

}
