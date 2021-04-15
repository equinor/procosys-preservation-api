using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags;

namespace Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags
{
    public class PagingDto
    {
        public int Page { get; set; } = GetTagsQuery.DefaultPage;
        public int Size { get; set; } = GetTagsQuery.DefaultPagingSize;
    }
}
