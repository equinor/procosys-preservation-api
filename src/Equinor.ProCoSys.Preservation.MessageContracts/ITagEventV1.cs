namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface ITagEventV1 : IIntegrationEvent
{
    string Remark { get; }
    DateTime? NextDueTimeUtc { get; }

    Guid StepGuid { get; }

    string TagType { get; }
    string StorageArea { get; }

    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }

    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }

    string Status { get; }

    Guid? CommPkgGuid { get; }
    Guid? McPkgGuid { get; }

    bool IsVoided { get; }
}
