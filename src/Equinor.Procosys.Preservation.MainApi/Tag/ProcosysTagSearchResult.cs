using System.Collections.Generic;
using System.Diagnostics;

namespace Equinor.Procosys.Preservation.MainApi.Tag
{
    [DebuggerDisplay("{Items.Count} of {MaxAvailable} available tags")]
    public class ProcosysTagSearchResult
    {
        public int MaxAvailable { get; set; }
        public List<ProcosysTagOverview> Items { get; set; }
    }
}
