using System;
using System.Text.Json.Serialization;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class RequirementDefinitionEvent : IRequirementDefinitionEventV1
{
    public Guid Guid { get; init; }
    public string Plant { get; init; }

    [JsonIgnore] //ProjectName isnt needed for RequirementDefinition but is required for IIntegrationEvent
    public string ProjectName { get; init; } = null;

    public Guid RequirementTypeGuid { get; init; }

    public string Title { get; init; }
    public bool IsVoided { get; init; }
    public int DefaultIntervalWeeks { get; init; }
    public string Usage { get; init; }
    public int SortKey { get; init; }
    public bool NeedsUserInput { get; init; }

    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init; }

}
