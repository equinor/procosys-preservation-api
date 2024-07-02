namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IPreservationPeriodEventV1 : IIntegrationEvent
{
    Guid TagRequirementGuid { get; }
    string Status { get; }
    DateTime DueTimeUtc { get; }
    string Comment { get; }
    Guid PreservationRecordGuid { get; }
    DateTime CreatedAtUtc { get; }
    int CreatedById { get; }
    DateTime? ModifiedAtUtc { get; }
    int? ModifiedById { get; }
    int TagRequirementId { get; }
}
