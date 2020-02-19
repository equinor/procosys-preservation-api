using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class ActionDto
    {
        public ActionDto(
            int id, 
            string description,
            DateTime? dueTimeUtc,
            List<ActionCommentDto> comments)
        {
            Id = id;
            Description = description;
            DueTimeUtc = dueTimeUtc;
            Comments = comments ?? new List<ActionCommentDto>();
        }

        public int Id { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
        public List<ActionCommentDto> Comments { get; }
    }
}
