namespace Equinor.ProCoSys.Preservation.Query.TagApiQueries.SearchTags
{
    public class PCSTagDto
    {
        public PCSTagDto(
            string tagNo, 
            string description, 
            string purchaseOrderTitle,
            string commPkgNo,
            string mcPkgNo,
            string tagFunctionCode,
            string registerCode,
            string mccrResponsibleCodes,
            bool isPreserved)
        {
            TagNo = tagNo;
            Description = description;
            PurchaseOrderTitle = purchaseOrderTitle;
            CommPkgNo = commPkgNo;
            McPkgNo = mcPkgNo;
            RegisterCode = registerCode;
            TagFunctionCode = tagFunctionCode;
            MccrResponsibleCodes = mccrResponsibleCodes;
            IsPreserved = isPreserved;
        }

        public string TagNo { get; }
        public string Description { get; }
        public string PurchaseOrderTitle { get; }
        public string CommPkgNo { get; }
        public string McPkgNo { get; }
        public string RegisterCode { get; }
        public string TagFunctionCode { get; }
        public string MccrResponsibleCodes { get; }
        public bool IsPreserved { get; }
    }
}
