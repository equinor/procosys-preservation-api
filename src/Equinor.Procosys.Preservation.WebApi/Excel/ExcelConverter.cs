using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;

namespace Equinor.Procosys.Preservation.WebApi.Excel
{
    public class ExcelConverter : IExcelConverter
    {
        private readonly IPlantProvider _plantProvider;

        public ExcelConverter(IPlantProvider plantProvider) => _plantProvider = plantProvider;

        public MemoryStream Convert(ExportDto dto)
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
