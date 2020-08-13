using System;
using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportDto
    {
        public ExportDto(
            ActionStatus? actionStatus,
            string areaCode,
            string calloffNo,
            string disciplineCode,
            bool isVoided,
            string mode,
            string purchaseOrderNo,
            IEnumerable<string> requirementTitles,
            string responsibleCode,
            string status,
            string tagDescription,
            string tagNo)
        {
            ActionStatus = actionStatus;
            AreaCode = areaCode;
            CalloffNo = calloffNo;
            Description = tagDescription;
            DisciplineCode = disciplineCode;
            IsVoided = isVoided;
            Mode = mode;
            PurchaseOrderNo = purchaseOrderNo;
            TagNo = tagNo;
            ResponsibleCode = responsibleCode;
            Status = status;
            RequirementTitles = requirementTitles ?? throw new ArgumentNullException(nameof(requirementTitles));
        }

        public ActionStatus? ActionStatus { get; }
        public string AreaCode { get; }
        public string CalloffNo { get; }
        public string Description { get; }
        public string DisciplineCode { get; }
        public bool IsVoided { get; }
        public string Mode { get; }
        public string PurchaseOrderNo { get; }
        public IEnumerable<string> RequirementTitles { get; }
        public string ResponsibleCode { get; }
        public string Status { get; }
        public string TagNo { get; }
    }
}
