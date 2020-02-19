using System;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class ActionCommentDto
    {
        public ActionCommentDto(int commentId, DateTime commentedAt, PersonDto commentedBy)
        {
            Id = commentId;
            CommentedAt = commentedAt;
            CommentedBy = commentedBy;
        }

        public int Id { get; }
        public DateTime CommentedAt { get; }
        public PersonDto CommentedBy { get; }
    }
}
