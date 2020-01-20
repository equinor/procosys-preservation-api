using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate
{
    public class PreservationRecord : SchemaEntityBase
    {
        public const int CommentLengthMax = 2048;

        protected PreservationRecord()
            : base(null)
        {
        }
        
        public PreservationRecord(
            string schema,
            DateTime preservedAtUtc,
            Person preservedBy,
            bool bulkPreserved,
            string comment) : base(schema)
        {
            if (preservedBy == null)
            {
                throw new ArgumentNullException(nameof(preservedBy));
            }
            if (preservedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(preservedAtUtc)} is not Utc");
            }
            PreservedAtUtc = preservedAtUtc;
            PreservedByPersonId = preservedBy.Id;
            Comment = comment;
            BulkPreserved = bulkPreserved;
        }

        public DateTime PreservedAtUtc { get; private set; }
        public int PreservedByPersonId { get; private set; }
        public string Comment { get; private set; }
        public bool BulkPreserved { get; private set; }
    }
}
