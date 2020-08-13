using System.Collections.Generic;
using System.IO;
using Equinor.Procosys.Preservation.Query.GetTagsQueries;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExcel;

namespace Equinor.Procosys.Preservation.WebApi.Excel
{
    public interface IExcelConverter
    {
        MemoryStream Convert(Filter filter, IEnumerable<TagDto> dtos);
        string GetFileName();
    }
}
