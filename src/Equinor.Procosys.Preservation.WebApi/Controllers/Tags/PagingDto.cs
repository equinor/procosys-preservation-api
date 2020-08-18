using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTags;

namespace Equinor.Procosys.Preservation.WebApi.Controllers.Tags
{
    public class PagingDto
    {
        public int Page { get; set; } = GetTagsQuery.DefaultPage;
        public int Size { get; set; } = GetTagsQuery.DefaultPagingSize;
    }
}
