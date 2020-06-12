namespace Equinor.Procosys.Preservation.Query.GetPreservationRecord
{
    public class PreservationRecordDto
    {
        public PreservationRecordDto(
            int id,
            bool bulkPreserved)
        {
            Id = id;
            BulkPreserved = bulkPreserved;
        }

        public int Id { get; }
        public bool BulkPreserved { get; }
    }
}
