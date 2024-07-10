namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IStepEventV1 : IIntegrationEvent
{
    Guid ModeGuid { get; }
    Guid ResponsibleGuid { get; }

    string Title { get; }
    bool IsSupplierStep { get; }
    string AutoTransferMethod { get; }
    int SortKey { get; }
    bool IsVoided { get; }

    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }
    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }
}
