using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Action : SchemaEntityBase
    {
        public const int DescriptionLengthMax = 4096;

        protected Action() : base(null)
        {
        }

        public Action(string schema, string description, DateTime? dueTimeUtc)
            : base(schema)
        {
            Description = description;
            SetDueTime(dueTimeUtc);
        }

        public string Description { get; private set; }

        public DateTime? DueTimeUtc { get; private set; }

        public bool Cleared { get; private set; }

        public int? ClearedById { get; private set; }

        public void SetDueTime(DateTime? dueTimeUtc)
        {
            if (dueTimeUtc.HasValue && dueTimeUtc.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(dueTimeUtc)} is not Utc");
            }
            DueTimeUtc = dueTimeUtc;
        }

        public void Clear(Person clearedBy)
        {
            if (clearedBy == null)
            {
                throw new ArgumentNullException(nameof(clearedBy));
            }

            Cleared = true;
            ClearedById = clearedBy.Id;
        }
    }
}
