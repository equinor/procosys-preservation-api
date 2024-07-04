namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface ITagEventV1 : IIntegrationEvent
{
    string Description { get; }
    string Remark { get; }
    DateTime? NextDueTimeUtc { get; }

    int StepId { get; }
    string DisciplineCode { get; }
    string AreaCode { get; }
    string TagFunctionCode { get; }
    string PurchaseOrderNo { get; }
    string TagType { get; }
    string StorageArea { get; }
    string AreaDescription { get; }
    string DisciplineDescription { get; }

    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }

    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }

    string Status { get; }

    Guid? CommPkgGuid { get; }
    Guid? McPkgGuid { get; }

    bool IsVoided { get; }
    bool IsVoidedInSource { get; }
    bool IsDeletedInSource { get; }
}
