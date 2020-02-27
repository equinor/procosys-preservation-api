using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class PreservationRecord : SchemaEntityBase, ICreationAuditable, IModificationAuditable
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
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime? ModifiedAtUtc { get; private set; }
        public int? ModifiedById { get; private set; }

        public void SetCreated(DateTime createdAtUtc, Person createdBy)
        {
            if (createdAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(createdAtUtc)} is not UTC");
            }

            CreatedAtUtc = createdAtUtc;
            CreatedById = createdBy.Id;
        }

        public void SetModified(DateTime modifiedAtUtc, Person modifiedBy)
        {
            if (modifiedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(modifiedAtUtc)} is not UTC");
            }

            ModifiedAtUtc = modifiedAtUtc;
            ModifiedById = modifiedBy.Id;
        }
    }
}
