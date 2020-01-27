using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class PreservationPeriod : SchemaEntityBase
    {
        public const int CommentLengthMax = 2048;

        protected PreservationPeriod()
            : base(null)
        {
        }
        
        public PreservationPeriod(
            string schema,
            DateTime dueTimeUtc,
            PreservationPeriodStatus status) : base(schema)
        {
            if (dueTimeUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(dueTimeUtc)} is not Utc");
            }

            if (status != PreservationPeriodStatus.NeedUserInput && status != PreservationPeriodStatus.ReadyToPreserve)
            {
                throw new ArgumentException($"{nameof(dueTimeUtc)} {status} is an illegal initial status for a {nameof(PreservationPeriod)}");
            }
            DueImeUtc = dueTimeUtc;
            Status = status;
        }

        public PreservationPeriodStatus Status { get; private set; }
        public DateTime DueImeUtc { get; private set; }
        public string Comment { get; set; }
        public PreservationRecord PreservationRecord { get; set; }

        internal void Preserve(DateTime preservedAtUtc, Person preservedBy, bool bulkPreserved)
        {
            if (preservedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(preservedAtUtc)} is not Utc");
            }

            if (Status != PreservationPeriodStatus.ReadyToPreserve)
            {
                throw new Exception($"{Status} is an illegal status for {nameof(PreservationPeriod)}. Can't preserve");
            }

            if (PreservationRecord != null)
            {
                throw new Exception($"{nameof(PreservationPeriod)} already have a {nameof(PreservationRecord)}. Can't preserve");
            }

            PreservationRecord = new PreservationRecord(base.Schema, preservedAtUtc, preservedBy, bulkPreserved);
        }
    }
}
