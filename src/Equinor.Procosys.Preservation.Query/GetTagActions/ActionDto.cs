using System;

namespace Equinor.Procosys.Preservation.Query.GetTagActions
{
    public class ActionDto
    {
        public ActionDto(
            int id, 
            string title,
            DateTime? dueTimeUtc,
            bool isClosed)
        {
            Id = id;
            Title = title;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
        }

        public int Id { get; }
        public string Title { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
    }
}
