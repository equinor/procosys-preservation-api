using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class PreservationPeriod : SchemaEntityBase
    {
        private readonly List<FieldValue> _fieldValues = new List<FieldValue>();

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

            if (status != PreservationPeriodStatus.NeedsUserInput && status != PreservationPeriodStatus.ReadyToBePreserved)
            {
                throw new ArgumentException($"{status} is an illegal initial status for a {nameof(PreservationPeriod)}");
            }
            DueTimeUtc = dueTimeUtc;
            Status = status;
        }

        public PreservationPeriodStatus Status { get; private set; }
        public DateTime DueTimeUtc { get; private set; }
        public string Comment { get; set; }
        public PreservationRecord PreservationRecord { get; private set; }
        public IReadOnlyCollection<FieldValue> FieldValues => _fieldValues.AsReadOnly();

        public void AddFieldValue(FieldValue fieldValue)
        {
            if (fieldValue == null)
            {
                throw new ArgumentNullException(nameof(fieldValue));
            }

            _fieldValues.Add(fieldValue);
        }

        public void Preserve(DateTime preservedAtUtc, Person preservedBy, bool bulkPreserved)
        {
            if (PreservationRecord != null)
            {
                throw new Exception($"{nameof(PreservationPeriod)} already has a {nameof(PreservationRecord)}. Can't preserve");
            }

            if (Status != PreservationPeriodStatus.ReadyToBePreserved)
            {
                throw new Exception($"{Status} is an illegal status for {nameof(PreservationPeriod)}. Can't preserve");
            }

            if (preservedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(preservedAtUtc)} is not Utc");
            }

            Status = PreservationPeriodStatus.Preserved;
            PreservationRecord = new PreservationRecord(base.Schema, preservedAtUtc, preservedBy, bulkPreserved);
        }
    }
}
