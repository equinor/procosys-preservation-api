using System.Collections.Generic;
using System.Linq;

namespace Equinor.Procosys.Preservation.Command.MainApi
{
    public class MainTagSearchDto
    {
        public int MaxAvailable { get; set; }
        public IEnumerable<MainTagDto> Items { get; set; }

        public override string ToString() => $"{Items.Count()} of {MaxAvailable} available tags";
    }
}
