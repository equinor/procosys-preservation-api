using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags
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
