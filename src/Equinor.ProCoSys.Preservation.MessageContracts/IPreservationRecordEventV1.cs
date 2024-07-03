namespace Equinor.ProCoSys.Preservation.MessageContracts;

public interface IPreservationRecordEventV1
{
    DateTime PreservedAtUtc { get; set; }
    int PreservedById { get; set; }
    bool BulkPreserved { get; set; }
}
