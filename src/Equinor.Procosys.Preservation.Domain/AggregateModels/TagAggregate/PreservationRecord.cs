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
        
        public PreservationRecord(string schema, Requirement requirement, DateTime currentTime) : base(schema)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            if (currentTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(currentTime)} is not Utc");
            }

            RequirementId = requirement.Id;
            NextDueTimeUtc = currentTime.AddDays(7*requirement.IntervalWeeks);
        }

        public int RequirementId { get; private set; }
        public DateTime NextDueTimeUtc { get; private set; }
        public bool? BulkPreserved { get; set; }
        public DateTime? PreservedAtUtc { get; set; }
        public int? PreservedBy { get; set; }
        public string Comment { get; set; }
 
        public void Preserve(Person preservedBy, string comment, DateTime currentTime) => Preserve(preservedBy, comment, false, currentTime);

        public void BulkPreserve(Person preservedBy, DateTime currentTime) => Preserve(preservedBy, null, true, currentTime);

        private void Preserve(Person preservedBy, string comment, bool bulkPreserve, DateTime currentTime)
        {
            if (preservedBy == null)
            {
                throw new ArgumentNullException(nameof(preservedBy));
            }

            if (currentTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(currentTime)} is not Utc");
            }

            PreservedBy = preservedBy.Id;
            PreservedAtUtc = currentTime;
            Comment = comment;
            BulkPreserved = bulkPreserve;
        }
    }
}
