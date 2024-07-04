using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class FieldEvent : IFieldEventV1
{
    public Guid RequirementDefinitionGuid { get; init; }
    public string Label { get; init; }
    public string Unit { get; init; }
    public int SortKey { get; init; }
    public string FieldType { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public Guid CreatedByGuid { get; init; }
    public DateTime? ModifiedAtUtc { get; init; }
    public Guid? ModifiedByGuid { get; init; }
    public Guid Guid { get; init; }
    public string Plant { get; init; }
    public string ProjectName { get; init; }
}
