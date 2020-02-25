using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
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
            bool bulkPreserved) : base(schema)
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
            PreservedById = preservedBy.Id;
            BulkPreserved = bulkPreserved;
        }

        public DateTime PreservedAtUtc { get; private set; }
        public int PreservedById { get; private set; }
        public bool BulkPreserved { get; private set; }
    }
}
