using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.GetTags
{
    public class TagsResult
    {
        public TagsResult(int maxAvailable, IEnumerable<TagDto> tags)
        {
            MaxAvailable = maxAvailable;
            Tags = tags ?? new List<TagDto>();
        }

        public int MaxAvailable { get; }
        public IEnumerable<TagDto> Tags { get; }
    }
}
