using System;

namespace Equinor.ProCoSys.Preservation.Query.TagApiQueries.PreservedTags
{
    public class ProcosysPreservedTagDto
    {
        public ProcosysPreservedTagDto(
            long id,
            string tagNo, 
            string description, 
            string purchaseOrderTitle,
            string commPkgNo,
            string mcPkgNo,
            string tagFunctionCode,
            string registerCode,
            string mccrResponsibleCodes, 
            string preservationRemark, 
            string storageArea, 
            string modeCode, 
            bool heating, 
            bool special,
            DateTime? nextUpcommingDueTime,
            DateTime? startDate,
            bool isPreserved)
        {
            Id = id;
            TagNo = tagNo;
            Description = description;
            PurchaseOrderTitle = purchaseOrderTitle;
            CommPkgNo = commPkgNo;
            McPkgNo = mcPkgNo;
            RegisterCode = registerCode;
            TagFunctionCode = tagFunctionCode;
            MccrResponsibleCodes = mccrResponsibleCodes;
            PreservationRemark = preservationRemark;
            StorageArea = storageArea;
            ModeCode = modeCode;
            Heating = heating;
            Special = special;
            NextUpcommingDueTime = nextUpcommingDueTime;
            StartDate = startDate;
            IsPreserved = isPreserved;
        }

        public long Id { get; }
        public string CommPkgNo { get; }
        public string Description { get; }
        public string McPkgNo { get; }
        public string PurchaseOrderTitle { get; }
        public string RegisterCode { get; }
        public string TagFunctionCode { get; }
        public string TagNo { get; }
        public string MccrResponsibleCodes { get; }
        public string PreservationRemark { get; }
        public string StorageArea { get; }
        public string ModeCode { get; }
        public bool Heating { get; }
        public bool Special { get; }
        public DateTime? NextUpcommingDueTime { get; }
        public DateTime? StartDate { get; }
        public bool IsPreserved { get; }
    }
}
