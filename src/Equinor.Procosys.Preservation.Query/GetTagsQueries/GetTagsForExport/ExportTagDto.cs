using System;
using System.Collections.Generic;

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
            string purchaseOrderTitle,
            IEnumerable<string> requirementTitles,
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
            PurchaseOrderTitle = purchaseOrderTitle;
            TagNo = tagNo;
            ResponsibleCode = responsibleCode;
            Status = status;
            RequirementTitles = requirementTitles ?? throw new ArgumentNullException(nameof(requirementTitles));
        }

        public string ActionStatus { get; }
        public string AreaCode { get; }
        public string Description { get; }
        public string DisciplineCode { get; }
        public bool IsVoided { get; }
        public string Mode { get; }
        public string PurchaseOrderTitle { get; }
        public IEnumerable<string> RequirementTitles { get; }
        public string ResponsibleCode { get; }
        public string Status { get; }
        public string TagNo { get; }
    }
}
