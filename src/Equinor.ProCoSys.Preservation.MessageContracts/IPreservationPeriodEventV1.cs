namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IPreservationPeriodEventV1 : IIntegrationEvent
{
    Guid TagRequirementGuid { get; }
    string Status { get; }
    DateTime DueTimeUtc { get; }
    string Comment { get; }
    Guid? PreservationRecordGuid { get; }
    DateTime CreatedAtUtc { get; }
    Guid CreatedByGuid { get; }
    DateTime? ModifiedAtUtc { get; }
    Guid? ModifiedByGuid { get; }
    DateTime? PreservedAtUtc { get; }
    Guid? PreservedByGuid { get; }
    bool? BulkPreserved { get; }
}
