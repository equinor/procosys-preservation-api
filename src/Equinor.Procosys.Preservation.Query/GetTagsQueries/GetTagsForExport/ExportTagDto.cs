using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportTagDto
    {
        public ExportTagDto(
            List<ExportActionDto> actions,
            string actionStatus,
            int actionsCount,
            string areaCode,
            int attachmentsCount,
            string commPkgNo,
            string disciplineCode,
            bool isVoided,
            string journey,
            string mcPkgNo,
            string mode,
            string nextDueAsYearAndWeek,
            int? nextDueWeeks,
            int openActionsCount,
            int overdueActionsCount,
            string purchaseOrderTitle,
            string remark,
            string requirementTitles,
            string responsibleCode,
            string status,
            string step,
            string storageArea,
            string tagDescription,
            string tagNo)
        {
            Actions = actions;
            ActionStatus = actionStatus;
            ActionsCount = actionsCount;
            AreaCode = areaCode;
            AttachmentsCount = attachmentsCount;
            CommPkgNo = commPkgNo;
            Description = tagDescription;
            DisciplineCode = disciplineCode;
            IsVoided = isVoided;
            Journey = journey;
            McPkgNo = mcPkgNo;
            Mode = mode;
            NextDueWeeks = nextDueWeeks;
            OpenActionsCount = openActionsCount;
            OverdueActionsCount = overdueActionsCount;
            PurchaseOrderTitle = purchaseOrderTitle;
            TagNo = tagNo;
            NextDueAsYearAndWeek = nextDueAsYearAndWeek;
            Remark = remark;
            RequirementTitles = requirementTitles;
            ResponsibleCode = responsibleCode;
            Status = status;
            Step = step;
            StorageArea = storageArea;

            History = new List<ExportHistoryDto>();
        }

        public List<ExportActionDto> Actions { get; }
        public string ActionStatus { get; }
        public int ActionsCount { get; }
        public string AreaCode { get; }
        public int AttachmentsCount { get; }
        public string CommPkgNo { get; }
        public string Description { get; }
        public string DisciplineCode { get; }
        public List<ExportHistoryDto> History { get; }
        public bool IsVoided { get; }
        public string Journey { get; }
        public string McPkgNo { get; }
        public string Mode { get; }
        public string NextDueAsYearAndWeek { get; }
        /// <summary>
        /// NextDueWeeks shifts at Monday night regardless of where in week the NextDueTimeUtc / current time is
        /// </summary>
        public int? NextDueWeeks { get; }
        public int OpenActionsCount { get; }
        public int OverdueActionsCount { get; }
        public string PurchaseOrderTitle { get; }
        public string RequirementTitles { get; }
        public string Remark { get; }
        public string ResponsibleCode { get; }
        public string Step { get; }
        public string StorageArea { get; }
        public string Status { get; }
        public string TagNo { get; }
    }
}
