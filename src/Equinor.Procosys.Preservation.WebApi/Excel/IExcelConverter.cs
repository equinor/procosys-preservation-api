using System.Collections.Generic;
using System.IO;
using Equinor.Procosys.Preservation.Query.GetTagsQueries;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;

namespace Equinor.Procosys.Preservation.WebApi.Excel
{
    // todo unit test
    public interface IExcelConverter
    {
        MemoryStream Convert(Filter filter, IEnumerable<ExportDto> dtos);
        string GetFileName();
    }
}
