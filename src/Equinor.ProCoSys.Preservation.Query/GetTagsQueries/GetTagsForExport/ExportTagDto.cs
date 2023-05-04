using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportTagDto
    {
        public ExportTagDto(
            int tagId,
            List<ExportActionDto> actions,
            List<ExportRequirementDto> requirements,
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
            int openActionsCount,
            int overdueActionsCount,
            string purchaseOrderTitle,
            string remark,
            string responsibleCode,
            string status,
            string step,
            string storageArea,
            string tagDescription,
            string tagNo)
        {
            TagId = tagId;
            Actions = actions;
            Requirements = requirements;
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
            OpenActionsCount = openActionsCount;
            OverdueActionsCount = overdueActionsCount;
            PurchaseOrderTitle = purchaseOrderTitle;
            TagNo = tagNo;
            Remark = remark;
            ResponsibleCode = responsibleCode;
            Status = status;
            Step = step;
            StorageArea = storageArea;

            History = new List<ExportHistoryDto>();
        }

        public int TagId { get; }
        public List<ExportActionDto> Actions { get; }
        public List<ExportRequirementDto> Requirements { get; }
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
        public int OpenActionsCount { get; }
        public int OverdueActionsCount { get; }
        public string PurchaseOrderTitle { get; }
        public string Remark { get; }
        public string ResponsibleCode { get; }
        public string Step { get; }
        public string StorageArea { get; }
        public string Status { get; }
        public string TagNo { get; }
    }
}
