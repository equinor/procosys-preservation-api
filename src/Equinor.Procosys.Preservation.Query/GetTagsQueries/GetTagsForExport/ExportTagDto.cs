namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportTagDto
    {
        public ExportTagDto(
            string actionStatus,
            string areaCode,
            string disciplineCode,
            bool isVoided,
            string mode,
            string nextDueAsYearAndWeek,
            int? nextDueWeeks,
            string purchaseOrderTitle,
            string requirements,
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
            Mode = mode;
            NextDueWeeks = nextDueWeeks;
            PurchaseOrderTitle = purchaseOrderTitle;
            TagNo = tagNo;
            NextDueAsYearAndWeek = nextDueAsYearAndWeek;
            ResponsibleCode = responsibleCode;
            Status = status;
            Requirements = requirements;
        }

        public string ActionStatus { get; }
        public string AreaCode { get; }
        public string Description { get; }
        public string DisciplineCode { get; }
        public bool IsVoided { get; }
        public string Mode { get; }
        public string NextDueAsYearAndWeek { get; }
        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }
        public string PurchaseOrderTitle { get; }
        public string Requirements { get; }
        public string ResponsibleCode { get; }
        public string Status { get; }
        public string TagNo { get; }
    }
}
