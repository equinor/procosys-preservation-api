namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IResponsibleEventV1 : IIntegrationEvent
{
    bool IsVoided { get; }

    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }
    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }
}
