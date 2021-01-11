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
            string title)
        {
            Id = id;
            ClosedAtUtc = closedAtUtc;
            Description = description;
            DueTimeUtc = dueTimeUtc;
            Title = title;
        }

        public int Id { get; }
        public DateTime? ClosedAtUtc { get; }
        public string Description { get; }
        public DateTime? DueTimeUtc { get; }
        public string Title { get; }
    }
}
