using System.IO;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;

namespace Equinor.Procosys.Preservation.WebApi.Excel
{
    public interface IExcelConverter
    {
        MemoryStream Convert(ExportDto dto);
        string GetFileName();
    }
}
