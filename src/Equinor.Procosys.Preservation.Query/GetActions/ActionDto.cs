using System;

namespace Equinor.Procosys.Preservation.Query.GetActions
{
    public class ActionDto
    {
        public ActionDto(
            int id,
            string title,
            DateTime? dueTimeUtc,
            bool isClosed,
            string rowVersion)
        {
            Id = id;
            Title = title;
            DueTimeUtc = dueTimeUtc;
            IsClosed = isClosed;
            RowVersion = rowVersion;
        }

        public int Id { get; }
        public string Title { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsClosed { get; }
        public string RowVersion { get; }
    }
}
