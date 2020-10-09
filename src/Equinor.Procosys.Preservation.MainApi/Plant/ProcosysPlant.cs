using System.Diagnostics;

namespace Equinor.Procosys.Preservation.MainApi.Plant
{
    [DebuggerDisplay("{Title} {Id} {HasAccess}")]
    public class ProcosysPlant
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool HasAccess { get; set; }
    }
}
