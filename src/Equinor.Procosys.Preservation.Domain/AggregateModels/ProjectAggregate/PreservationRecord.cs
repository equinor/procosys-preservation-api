using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class PreservationRecord : SchemaEntityBase, ICreationAuditable
    {
        public const int CommentLengthMax = 2048;

        protected PreservationRecord()
            : base(null)
        {
        }
        
        public PreservationRecord(
            string schema,
            Person preservedBy,
            bool bulkPreserved) : base(schema)
        {
            if (preservedBy == null)
            {
                throw new ArgumentNullException(nameof(preservedBy));
            }
            PreservedAtUtc = TimeService.UtcNow;
            PreservedById = preservedBy.Id;
            BulkPreserved = bulkPreserved;
        }

        public DateTime PreservedAtUtc { get; private set; }
        public int PreservedById { get; private set; }
        public bool BulkPreserved { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int CreatedById { get; private set; }

        public void SetCreated(Person createdBy)
        {
            CreatedAtUtc = TimeService.UtcNow;
            if (createdBy == null)
            {
                throw new ArgumentNullException(nameof(createdBy));
            }
            CreatedById = createdBy.Id;
        }
    }
}
