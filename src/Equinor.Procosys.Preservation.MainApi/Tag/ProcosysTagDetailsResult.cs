using System.Diagnostics;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{

    [DebuggerDisplay("{Tag}")]
    public class ProcosysTagDetailsResult
    {
        public ProcosysTagDetails Tag { get; set; }
    }
}
