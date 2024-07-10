namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IResponsibleEventV1 : IIntegrationEvent
{
    string Code { get; }
    string Description { get; }
    bool IsVoided { get; }

    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }
    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }
}
