using System;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportActionDto
    {
        public ExportActionDto(
            int id, 
            string title,
            string description, 
            bool isOverDue, 
            DateTime? dueTimeUtc, 
            DateTime? closedAtUtc)
        {
            Id = id;
            Title = title;
            Description = description;
            IsOverDue = isOverDue;
            DueTimeUtc = dueTimeUtc;
            ClosedAtUtc = closedAtUtc;
        }

        public int Id { get; }
        public string Title { get; }
        public string Description { get; }
        public bool IsOverDue { get; }
        public DateTime? DueTimeUtc { get; }
        public DateTime? ClosedAtUtc { get; }
    }
}
