namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IPreservationRecordEventV1 : IIntegrationEvent
{
    DateTime PreservedAtUtc { get; set; }
    int PreservedById { get; set; }
    bool BulkPreserved { get; set; }
    DateTime CreatedAtUtc { get; set; }
    int CreatedById { get; set; }
    Guid Guid { get; set; }
}
