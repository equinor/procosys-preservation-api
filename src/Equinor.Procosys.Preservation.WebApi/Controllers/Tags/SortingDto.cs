using Equinor.Procosys.Preservation.Query.GetTags;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class SortingDto
    {
        public SortingDirection Direction { get; set; } = GetTagsQuery.DefaultSortingDirection;
        public SortingProperty Property { get; set; } = GetTagsQuery.DefaultSortingProperty;
    }
}
