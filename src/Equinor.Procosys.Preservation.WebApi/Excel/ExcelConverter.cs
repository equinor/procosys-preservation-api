using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;

namespace Equinor.Procosys.Preservation.WebApi.Excel
{
    public class ExcelConverter : IExcelConverter
    {
        public MemoryStream Convert(ExportDto dto)
        {
            // see https://github.com/ClosedXML/ClosedXML for sample code
            var excelStream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                CreateFrontSheet(workbook, dto.UsedFilter);
                CreateTagSheet(workbook, dto.Tags);

                workbook.SaveAs(excelStream);
            }

            return excelStream;
        }

        private void CreateTagSheet(XLWorkbook workbook, IEnumerable<ExportTagDto> tags)
        {
            var colIdx = 0;

            var tagNoCol = ++colIdx;
            var descriptionCol = ++colIdx;
            var nextCol = ++colIdx;
            var dueCol = ++colIdx;
            var modeCol = ++colIdx;
            var poCol = ++colIdx;
            var areaCol = ++colIdx;
            var respCol = ++colIdx;
            var discCol = ++colIdx;
            var statusCol = ++colIdx;
            var reqCol = ++colIdx;
            var actionCol = ++colIdx;
            var voidedCol = ++colIdx;

            var sheet = workbook.Worksheets.Add("Tags");

            var rowIdx = 0;
            var row = sheet.Row(++rowIdx);
            row.Style.Font.SetBold();
            row.Style.Font.SetFontSize(12);
            row.Cell(tagNoCol).Value = "Tag nr";
            row.Cell(descriptionCol).Value = "Tag description";
            row.Cell(nextCol).Value = "Next preservation";
            row.Cell(dueCol).Value = "Due (weeks)";
            row.Cell(modeCol).Value = "Mode";
            row.Cell(poCol).Value = "Purchase order";
            row.Cell(areaCol).Value = "Area";
            row.Cell(respCol).Value = "Responsible";
            row.Cell(discCol).Value = "Discipline";
            row.Cell(statusCol).Value = "Status";
            row.Cell(reqCol).Value = "Requirements";
            row.Cell(actionCol).Value = "Action status";
            row.Cell(voidedCol).Value = "Is voided";

            foreach (var tag in tags)
            {
                row = sheet.Row(++rowIdx);

                row.Cell(tagNoCol).SetValue(tag.TagNo).SetDataType(XLDataType.Text);
                row.Cell(descriptionCol).SetValue(tag.Description).SetDataType(XLDataType.Text);
                row.Cell(nextCol).SetValue(tag.NextDueAsYearAndWeek).SetDataType(XLDataType.Text);
                if (tag.NextDueWeeks.HasValue)
                {
                    // The only number cell: NextDueWeeks
                    row.Cell(dueCol).SetValue(tag.NextDueWeeks.Value).SetDataType(XLDataType.Number);
                }
                row.Cell(modeCol).SetValue(tag.Mode).SetDataType(XLDataType.Text);
                row.Cell(poCol).SetValue(tag.PurchaseOrderTitle).SetDataType(XLDataType.Text);
                row.Cell(areaCol).SetValue(tag.AreaCode).SetDataType(XLDataType.Text);
                row.Cell(respCol).SetValue(tag.ResponsibleCode).SetDataType(XLDataType.Text);
                row.Cell(discCol).SetValue(tag.DisciplineCode).SetDataType(XLDataType.Text);
                row.Cell(statusCol).SetValue(tag.Status).SetDataType(XLDataType.Text);
                row.Cell(reqCol).SetValue(tag.RequirementTitles).SetDataType(XLDataType.Text);
                row.Cell(actionCol).SetValue(tag.ActionStatus).SetDataType(XLDataType.Text);
                row.Cell(voidedCol).SetValue(tag.IsVoided).SetDataType(XLDataType.Text);
            }

            const int minWidth = 10;
            const int maxWidth = 100;
            sheet.Columns(1, colIdx).AdjustToContents(1, rowIdx, minWidth, maxWidth);
        }

        private void CreateFrontSheet(XLWorkbook workbook, UsedFilterDto usedFilter)
        {
            var sheet = workbook.Worksheets.Add("Filters");
            var rowIdx = 0;
            var row = sheet.Row(++rowIdx);
            row.Style.Font.SetBold();
            row.Style.Font.SetFontSize(14);
            row.Cell(1).Value = "Export of preserved tags";

            rowIdx++;
            AddUsedFilter(sheet.Row(++rowIdx), "Plant", usedFilter.Plant, true);
            AddUsedFilter(sheet.Row(++rowIdx), "Project", usedFilter.ProjectName, true);
            AddUsedFilter(sheet.Row(++rowIdx), "Project description", usedFilter.ProjectDescription, true);

            rowIdx++;
            AddUsedFilter(sheet.Row(++rowIdx), "Filter values:", "", true);

            AddUsedFilter(sheet.Row(++rowIdx), "Tag number starts with", usedFilter.TagNoStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "Purchase order number starts with", usedFilter.PurchaseOrderNoStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "Calloff number starts with", usedFilter.CallOffStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "CommPkg number starts with", usedFilter.CommPkgNoStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "McPkg number starts with", usedFilter.McPkgNoStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "Storage area starts with", usedFilter.StorageAreaStartsWith);
            AddUsedFilter(sheet.Row(++rowIdx), "Preservation status", usedFilter.PreservationStatus);
            AddUsedFilter(sheet.Row(++rowIdx), "Preservation actions", usedFilter.ActionStatus);
            AddUsedFilter(sheet.Row(++rowIdx), "Voided/unvoided tags", usedFilter.VoidedFilter);
            AddUsedFilter(sheet.Row(++rowIdx), "Preservation dute date", usedFilter.DueFilters);
            AddUsedFilter(sheet.Row(++rowIdx), "Journeys", usedFilter.JourneyTitles);
            AddUsedFilter(sheet.Row(++rowIdx), "Modes", usedFilter.ModeTitles);
            AddUsedFilter(sheet.Row(++rowIdx), "Requirements", usedFilter.RequirementTypeTitles);
            AddUsedFilter(sheet.Row(++rowIdx), "Tag functions", usedFilter.TagFunctionCodes);
            AddUsedFilter(sheet.Row(++rowIdx), "Disciplines", usedFilter.DisciplineCodes);
            AddUsedFilter(sheet.Row(++rowIdx), "Responsibles", usedFilter.ResponsibleCodes);
            AddUsedFilter(sheet.Row(++rowIdx), "Areas", usedFilter.AreaCodes);
         
            sheet.Columns(1, 2).AdjustToContents();
        }

        private void AddUsedFilter(IXLRow row, string label, IEnumerable<string> values)
            => AddUsedFilter(row, label, string.Join(",", values));

        private void AddUsedFilter(IXLRow row, string label, string value, bool bold = false)
        {
            row.Cell(1).SetValue(label).SetDataType(XLDataType.Text);
            row.Cell(2).SetValue(value).SetDataType(XLDataType.Text);
            row.Style.Font.SetBold(bold);
        }

        public string GetFileName()=> $"PreservedTags-{DateTime.Now:yyyyMMdd-hhmmss}";
    }
}
