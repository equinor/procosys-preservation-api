using System;
using Equinor.ProCoSys.Preservation.Domain;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportRequirementDto
    {
        public ExportRequirementDto(
            int id,
            string requirementTitle,
            DateTime? nextDueTimeUtc,
            int? nextDueWeeks,
            string activeComment)
        {
            Id = id;
            RequirementTitle = requirementTitle;
            NextDueTimeUtc = nextDueTimeUtc;
            NextDueWeeks = nextDueWeeks;
            NextDueAsYearAndWeek = NextDueTimeUtc?.FormatAsYearAndWeekString();
            ActiveComment = activeComment;
        }

        public int Id { get; }
        public string RequirementTitle { get; }
        public DateTime? NextDueTimeUtc { get; }
        public string NextDueAsYearAndWeek { get; }

        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }

        public string ActiveComment { get; }
    }
}
