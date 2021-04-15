using System;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportHistoryDto
    {
        public ExportHistoryDto(
            int id,
            string description,
            DateTime createdAtUtc,
            string createdBy,
            int? dueInWeeks,
            string preservationDetails,
            string preservationComment)
        {
            Id = id;
            Description = description;
            CreatedAtUtc = createdAtUtc;
            CreatedBy = createdBy;
            DueInWeeks = dueInWeeks;
            PreservationDetails = preservationDetails;
            PreservationComment = preservationComment;
        }

        public int Id { get; }
        public string Description { get; }
        public DateTime CreatedAtUtc { get; }
        public string CreatedBy { get; }
        public int? DueInWeeks { get; }
        public string PreservationDetails { get; }
        public string PreservationComment { get; }
    }
}
