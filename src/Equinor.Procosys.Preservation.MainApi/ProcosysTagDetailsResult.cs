namespace Equinor.Procosys.Preservation.MainApi
{
    public class ProcosysTagDetailsResult
    {
        public ProcosysTagDetails Tag { get; set; }
    }

    public class ProcosysTagDetails
    {
        public string TagNo { get; set; }
        public string Description { get; set; }
        public string TagFunctionCode { get; set; }
        public string AreaCode { get; set; }
        public string DisciplineCode { get; set; }
        public string McPkgNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string CallOffNo { get; set; }

        public override string ToString() => TagNo;
    }
}
