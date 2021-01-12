using System;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportHistoryDto
    {
        public ExportHistoryDto(
            int id,
            string description,
            DateTime createdAtUtc,
            int? dueInWeeks)
        {
            Id = id;
            Description = description;
            CreatedAtUtc = createdAtUtc;
            DueInWeeks = dueInWeeks;
        }

        public int Id { get; }
        public string Description { get; }
        public DateTime CreatedAtUtc { get; }
        public int? DueInWeeks { get; }
    }
}
