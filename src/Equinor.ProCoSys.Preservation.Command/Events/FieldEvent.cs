using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class FieldEvent : IFieldEventV1
{
    public Guid RequirementDefinitionGuid { get; set; }
    public int FieldId { get; set; }
    public string Label { get; set; }
    public string Unit { get; set; }
    public int SortKey { get; set; }
    public bool? ShowPrevious { get; set; }
    public string FieldType { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int CreatedById { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public int? ModifiedById { get; set; }

    public Guid Guid { get; set; }
    public Guid ProCoSysGuid => Guid;
    public string Plant { get; set; }
    public string ProjectName { get; set; }
}
