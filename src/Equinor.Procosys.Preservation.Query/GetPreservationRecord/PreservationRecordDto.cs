namespace Equinor.Procosys.Preservation.Query.GetPreservationRecord
{
    public class PreservationRecordDto
    {
        public PreservationRecordDto(
            int id,
            bool bulkPreserved,
            string rowVersion)
        {
            Id = id;
            BulkPreserved = bulkPreserved;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public bool BulkPreserved { get; }
        public string RowVersion { get; }
    }
}
