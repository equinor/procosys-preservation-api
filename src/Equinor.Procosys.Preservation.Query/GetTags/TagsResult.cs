using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class TagsResult
    {
        public int MaxAvailable { get; set; }
        public IEnumerable<TagDto> Tags { get; set; }
    }
}
