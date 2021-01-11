using System;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportActionDto
    {
        public ExportActionDto(
            int id,
            DateTime? closedAtUtc,
            string description,
            DateTime? dueTimeUtc, 
            bool isOverDue,
            string title)
        {
            Id = id;
            ClosedAtUtc = closedAtUtc;
            Description = description;
            DueTimeUtc = dueTimeUtc;
            IsOverDue = isOverDue;
            Title = title;
        }

        public int Id { get; }
        public DateTime? ClosedAtUtc { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
        public bool IsOverDue { get; }
        public string Title { get; }
    }
}
