using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Equinor.Procosys.Preservation.Query.GetTagsQueries;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;

namespace Equinor.Procosys.Preservation.WebApi.Excel
{
    public class ExcelConverter : IExcelConverter
    {
        public MemoryStream Convert(Filter filter, IEnumerable<ExportDto> dtos)
        {
            var excelStream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                var frontSheet = workbook.Worksheets.Add("Filters");
                var tagsSheet = workbook.Worksheets.Add("Tags");

                workbook.SaveAs(excelStream);
            }

            return excelStream;
        }

        public string GetFileName()=> $"PreservedTags-{DateTime.Now:yyyyMMdd-hhmmss}";
    }
}
