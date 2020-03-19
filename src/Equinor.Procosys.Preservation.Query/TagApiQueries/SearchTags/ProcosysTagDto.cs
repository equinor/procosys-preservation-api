namespace Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags
{
    public class ProcosysTagDto
    {
        public ProcosysTagDto(string tagNo, string description, string purchaseOrderNumber, string commPkgNo, string mcPkgNo, string tagFunctionCode, string registerCode, bool isPreserved)
        {
            TagNo = tagNo;
            Description = description;
            PurchaseOrderNumber = purchaseOrderNumber;
            CommPkgNo = commPkgNo;
            McPkgNo = mcPkgNo;
            IsPreserved = isPreserved;
            RegisterCode = registerCode;
            TagFunctionCode = tagFunctionCode;
        }

        public string TagNo { get; }
        public string Description { get; }
        public string PurchaseOrderNumber { get; }
        public string CommPkgNo { get; }
        public string McPkgNo { get; }
        public bool IsPreserved { get; }
        public string RegisterCode { get; }
        public string TagFunctionCode { get; }
    }
}
