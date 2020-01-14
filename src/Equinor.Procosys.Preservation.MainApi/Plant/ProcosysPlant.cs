using System.Diagnostics;

namespace Equinor.Procosys.Preservation.MainApi.Plant
{
    [DebuggerDisplay("{Title} ({Id})")]
    public class ProcosysPlant
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
