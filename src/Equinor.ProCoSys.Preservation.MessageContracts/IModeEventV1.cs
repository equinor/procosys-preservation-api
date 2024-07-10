namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IModeEventV1 : IIntegrationEvent
{
    string Title { get; }
    bool IsVoided { get; }
    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }
    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }
    bool ForSupplier { get; }
}
