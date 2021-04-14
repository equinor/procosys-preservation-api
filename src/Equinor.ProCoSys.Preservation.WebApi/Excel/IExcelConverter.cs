using System.IO;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport;

namespace Equinor.ProCoSys.Preservation.WebApi.Excel
{
    public interface IExcelConverter
    {
        MemoryStream Convert(ExportDto dto);
        string GetFileName();
    }
}
