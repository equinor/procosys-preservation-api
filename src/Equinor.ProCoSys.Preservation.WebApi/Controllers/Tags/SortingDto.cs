using Equinor.ProCoSys.Preservation.Query.GetTagsQueries;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class SortingDto
    {
        public SortingDirection Direction { get; set; } = GetTagsQuery.DefaultSortingDirection;
        public SortingProperty Property { get; set; } = GetTagsQuery.DefaultSortingProperty;
    }
}
