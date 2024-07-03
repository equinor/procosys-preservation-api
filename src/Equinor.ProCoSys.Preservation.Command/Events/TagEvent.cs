using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class TagEvent : ITagEventV1
{
    public Guid Guid { get; set; }
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string Description { get; set; }
    public string Remark { get; set; }
    public DateTime? NextDueTimeUtc { get; set; }
    public int StepId { get; set; }
    public string DisciplineCode { get; set; }
    public string AreaCode { get; set; }
    public string TagFunctionCode { get; set; }
    public string PurchaseOrderNo { get; set; }
    public string TagType { get; set; }
    public string StorageArea { get; set; }
    public string AreaDescription { get; set; }
    public string DisciplineDescription { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int CreatedById { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public int? ModifiedById { get; set; }
    public string Status { get; set; }
    public Guid? CommPkgGuid { get; set; }
    public Guid? McPkgGuid { get; set; }
    public bool IsVoided { get; set; }
    public bool IsVoidedInSource { get; set; }
    public bool IsDeletedInSource { get; set; }
}
