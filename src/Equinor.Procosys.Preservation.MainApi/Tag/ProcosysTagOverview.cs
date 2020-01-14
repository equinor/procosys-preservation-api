using System.Diagnostics;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    [DebuggerDisplay("{TagNo}")]
    public class ProcosysTagOverview
    {
        public long Id { get; set; }
        public string TagNo { get; set; }
        public string Description { get; set; }
    }
}
