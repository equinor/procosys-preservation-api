namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportTagDto
    {
        public ExportTagDto(
            string actionStatus,
            string areaCode,
            string disciplineCode,
            bool isVoided,
            string journey,
            string step,
            string mode,
            string nextDueAsYearAndWeek,
            int? nextDueWeeks,
            string purchaseOrderTitle,
            string requirementTitles,
            string responsibleCode,
            string status,
            string tagDescription,
            string tagNo)
        {
            ActionStatus = actionStatus;
            AreaCode = areaCode;
            Description = tagDescription;
            DisciplineCode = disciplineCode;
            IsVoided = isVoided;
            Journey = journey;
            Step = step;
            Mode = mode;
            NextDueWeeks = nextDueWeeks;
            PurchaseOrderTitle = purchaseOrderTitle;
            TagNo = tagNo;
            NextDueAsYearAndWeek = nextDueAsYearAndWeek;
            ResponsibleCode = responsibleCode;
            Status = status;
            RequirementTitles = requirementTitles;
        }

        public string ActionStatus { get; }
        public string AreaCode { get; }
        public string Description { get; }
        public string DisciplineCode { get; }
        public bool IsVoided { get; }
        public string Journey { get; }
        public string Step { get; }
        public string Mode { get; }
        public string NextDueAsYearAndWeek { get; }
        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }
        public string PurchaseOrderTitle { get; }
        public string RequirementTitles { get; }
        public string ResponsibleCode { get; }
        public string Status { get; }
        public string TagNo { get; }
    }
}
