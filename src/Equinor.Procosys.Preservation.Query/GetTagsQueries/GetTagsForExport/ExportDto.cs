using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport
{
    public class ExportDto
    {
        public ExportDto(IEnumerable<ExportTagDto> tags, UsedFilterDto usedFilter)
        {
            UsedFilter = usedFilter;
            Tags = tags ?? new List<ExportTagDto>();
        }

        public IEnumerable<ExportTagDto> Tags { get; }
        public UsedFilterDto UsedFilter { get; }
    }
}
