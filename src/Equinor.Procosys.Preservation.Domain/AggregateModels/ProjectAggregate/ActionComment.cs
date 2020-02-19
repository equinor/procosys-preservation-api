using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate
{
    public class ActionComment : SchemaEntityBase
    {
        public const int CommentLengthMax = 4096;

        protected ActionComment() : base(null)
        {
        }

        public ActionComment(string schema, string comment, DateTime commentedAtUtc, Person commentedBy)
            : base(schema)
        {
            if (commentedBy == null)
            {
                throw new ArgumentNullException(nameof(commentedBy));
            }
            if (commentedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{nameof(commentedAtUtc)} is not Utc");
            }
            Comment = comment;
            CommentedAtUtc = commentedAtUtc;
            CommentedById = commentedBy.Id;
        }

        public string Comment { get; private set; }

        public DateTime CommentedAtUtc { get; private set; }

        public int CommentedById { get; private set; }
    }
}
