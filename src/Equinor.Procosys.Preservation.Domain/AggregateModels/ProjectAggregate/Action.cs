using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class Action : SchemaEntityBase
    {
        private readonly List<ActionComment> _actionComments = new List<ActionComment>();

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

        public DateTime? ClosedAtUtc { get; private set; }

        public int? ClosedById { get; private set; }

        public IReadOnlyCollection<ActionComment> ActionComments => _actionComments.AsReadOnly();
        
        public void AddComment(ActionComment actionComment)
        {
            if (actionComment == null)
            {
                throw new ArgumentNullException(nameof(actionComment));
            }

            _actionComments.Add(actionComment);
        }

        public void SetDueTime(DateTime? dueTimeUtc)
        {
            if (dueTimeUtc.HasValue && dueTimeUtc.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(dueTimeUtc)} is not Utc");
            }
            DueTimeUtc = dueTimeUtc;
        }

        public void Close(DateTime closedAtUtc, Person closedBy)
        {
            if (closedBy == null)
            {
                throw new ArgumentNullException(nameof(closedBy));
            }
            if (closedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(closedAtUtc)} is not Utc");
            }

            ClosedAtUtc = closedAtUtc;
            ClosedById = closedBy.Id;
        }
    }
}
