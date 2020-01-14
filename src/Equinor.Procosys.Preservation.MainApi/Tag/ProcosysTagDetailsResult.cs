using System.Diagnostics;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{

    [DebuggerDisplay("{Tag}")]
    public class ProcosysTagDetailsResult
    {
        public ProcosysTagDetails Tag { get; set; }
    }

    [DebuggerDisplay("{TagNo}")]
    public class ProcosysTagDetails
    {
        public string AreaCode { get; set; }
        public string CallOffNo { get; set; }
        public string CommPkgNo { get; set; }
        public string Description { get; set; }
        public string DisciplineCode { get; set; }
        public string McPkgNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string TagFunctionCode { get; set; }
        public string TagNo { get; set; }
    }
}
