using System.Collections.Generic;
using System.Linq;

namespace Equinor.Procosys.Preservation.MainApi
{
    public class ProcosysTagSearchResult
    {
        public int MaxAvailable { get; set; }
        public IEnumerable<ProcosysTagOverview> Items { get; set; }

        public override string ToString() => $"{Items.Count()} of {MaxAvailable} available tags";
    }

    public class ProcosysTagOverview
    {
        public long Id { get; set; }
        public string TagNo { get; set; }
        public string Description { get; set; }

        public override string ToString() => TagNo;
    }
}
