using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;

namespace Equinor.Procosys.Preservation.WebApi.Excel
{
    public class ExcelConverter : IExcelConverter
    {
        public static int TagNoCol = 1;
        public static int DescriptionCol = 2;
        public static int NextCol = 3;
        public static int DueCol = 4;
        public static int JourneyCol = 5;
        public static int StepCol = 6;
        public static int ModeCol = 7;
        public static int PoCol = 8;
        public static int AreaCol = 9;
        public static int RespCol = 10;
        public static int DiscCol = 11;
        public static int PresStatusCol = 12;
        public static int ReqCol = 13;
        public static int RemarkCol = 14;
        public static int StorageAreaCol = 15;
        public static int ActionStatusCol = 16;
        public static int CommPkgCol = 17;
        public static int McPkgCol = 18;
        public static int VoidedCol = 19;
        public static int LastCol = VoidedCol;

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
            var sheet = workbook.Worksheets.Add("Tags");

            var rowIdx = 0;
            var row = sheet.Row(++rowIdx);
            row.Style.Font.SetBold();
            row.Style.Font.SetFontSize(12);
            row.Cell(TagNoCol).Value = "Tag nr";
            row.Cell(DescriptionCol).Value = "Tag description";
            row.Cell(NextCol).Value = "Next preservation";
            row.Cell(DueCol).Value = "Due (weeks)";
            row.Cell(JourneyCol).Value = "Journey";
            row.Cell(StepCol).Value = "Step";
            row.Cell(ModeCol).Value = "Mode";
            row.Cell(PoCol).Value = "Purchase order";
            row.Cell(AreaCol).Value = "Area";
            row.Cell(RespCol).Value = "Responsible";
            row.Cell(DiscCol).Value = "Discipline";
            row.Cell(PresStatusCol).Value = "Status";
            row.Cell(ReqCol).Value = "Requirements";
            row.Cell(RemarkCol).Value = "Remark";
            row.Cell(StorageAreaCol).Value = "Storage area";
            row.Cell(ActionStatusCol).Value = "Action status";
            row.Cell(CommPkgCol).Value = "Comm pkg";
            row.Cell(McPkgCol).Value = "MC pkg";
            row.Cell(VoidedCol).Value = "Is voided";

            foreach (var tag in tags)
            {
                row = sheet.Row(++rowIdx);

                row.Cell(TagNoCol).SetValue(tag.TagNo).SetDataType(XLDataType.Text);
                row.Cell(DescriptionCol).SetValue(tag.Description).SetDataType(XLDataType.Text);
                row.Cell(NextCol).SetValue(tag.NextDueAsYearAndWeek).SetDataType(XLDataType.Text);
                if (tag.NextDueWeeks.HasValue)
                {
                    // The only number cell: NextDueWeeks
                    row.Cell(DueCol).SetValue(tag.NextDueWeeks.Value).SetDataType(XLDataType.Number);
                }
                row.Cell(JourneyCol).SetValue(tag.Journey).SetDataType(XLDataType.Text);
                row.Cell(StepCol).SetValue(tag.Step).SetDataType(XLDataType.Text);
                row.Cell(ModeCol).SetValue(tag.Mode).SetDataType(XLDataType.Text);
                row.Cell(PoCol).SetValue(tag.PurchaseOrderTitle).SetDataType(XLDataType.Text);
                row.Cell(AreaCol).SetValue(tag.AreaCode).SetDataType(XLDataType.Text);
                row.Cell(RespCol).SetValue(tag.ResponsibleCode).SetDataType(XLDataType.Text);
                row.Cell(DiscCol).SetValue(tag.DisciplineCode).SetDataType(XLDataType.Text);
                row.Cell(PresStatusCol).SetValue(tag.Status).SetDataType(XLDataType.Text);
                row.Cell(ReqCol).SetValue(tag.RequirementTitles).SetDataType(XLDataType.Text);
                row.Cell(RemarkCol).SetValue(tag.Remark).SetDataType(XLDataType.Text);
                row.Cell(StorageAreaCol).SetValue(tag.StorageArea).SetDataType(XLDataType.Text);
                row.Cell(ActionStatusCol).SetValue(tag.ActionStatus).SetDataType(XLDataType.Text);
                row.Cell(CommPkgCol).SetValue(tag.CommPkgNo).SetDataType(XLDataType.Text);
                row.Cell(McPkgCol).SetValue(tag.McPkgNo).SetDataType(XLDataType.Text);
                row.Cell(VoidedCol).SetValue(tag.IsVoided).SetDataType(XLDataType.Text);
            }

            const int minWidth = 10;
            const int maxWidth = 100;
            sheet.Columns(1, LastCol).AdjustToContents(1, rowIdx, minWidth, maxWidth);
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
