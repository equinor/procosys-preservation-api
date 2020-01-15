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
        
        public PreservationRecord(string schema, Requirement requirement, ITimeService timeService) : base(schema)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }
            if (timeService == null)
            {
                throw new ArgumentNullException(nameof(timeService));
            }

            RequirementId = requirement.Id;
            NextDueTime = timeService.GetCurrentTimeUtc().AddDays(7*requirement.IntervalWeeks);
        }

        public int RequirementId { get; private set; }
        public DateTime NextDueTime { get; private set; }
        public bool? BulkPreserved { get; set; }
        public DateTime? Preserved { get; set; }
        public int? PreservedBy { get; set; }
        public string Comment { get; set; }
 
        public void Preserve(Person preservedBy, string comment, ITimeService timeService)
        {
            if (preservedBy == null)
            {
                throw new ArgumentNullException(nameof(preservedBy));
            }
            if (timeService == null)
            {
                throw new ArgumentNullException(nameof(timeService));
            }

            PreservedBy = preservedBy.Id;
            Preserved = timeService.GetCurrentTimeUtc();
            Comment = comment;
        }   }
}
