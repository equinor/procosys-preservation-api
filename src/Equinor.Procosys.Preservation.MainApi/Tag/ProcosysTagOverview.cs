using System.Diagnostics;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    [DebuggerDisplay("{TagNo}")]
    public class ProcosysTagOverview
    {
        public long Id { get; set; }
        public string CommPkgNo { get; set; }
        public string Description { get; set; }
        public string McPkgNo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string RegisterCode { get; set; }
        public string TagFunctionCode { get; set; }
        public string TagNo { get; set; }
    }
}
