using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Excel
{
    public class ExcelConverter : IExcelConverter
    {
        public static class FrontSheetRows
        {
            public static int MainHeading = 1;
            public static int Blank1 = 2;
            public static int Plant = 3;
            public static int ProjectName = 4;
            public static int ProjectDesc = 5;
            public static int Blank2 = 6;
            public static int FilterHeading = 7;
            public static int Tag = 8;
            public static int PO = 9;
            public static int CO = 10;
            public static int CommPkg = 11;
            public static int McPkg = 12;
            public static int StorageArea = 13;
            public static int Status = 14;
            public static int Actions = 15;
            public static int Voided = 16;
            public static int Due = 17;
            public static int Journeys = 18;
            public static int Modes = 19;
            public static int Reqs = 20;
            public static int TF = 21;
            public static int Disc = 22;
            public static int Resp = 23;
            public static int Areas = 24;
        }

        public static class TagSheetColumns
        {
            public static int TagNo = 1;
            public static int Description = 2;
            public static int RequirementTitle = 3;
            public static int RequirementComment = 4;
            public static int RequirementNextInYearAndWeek = 5;
            public static int RequirementNextDueWeeks = 6;
            public static int Journey = 7;
            public static int Step = 8;
            public static int Mode = 9;
            public static int Po = 10;
            public static int Area = 11;
            public static int Resp = 12;
            public static int Disc = 13;
            public static int PresStatus = 14;
            public static int Remark = 15;
            public static int StorageArea = 16;
            public static int CommPkg = 17;
            public static int McPkg = 18;
            public static int ActionStatus = 19;
            public static int Actions = 20;
            public static int OpenActions = 21;
            public static int OverdueActions = 22;
            public static int Attachments = 23;
            public static int Voided = 24;
            public static int Last = Voided;
        }

        public static class ActionSheetColumns
        {
            public static int TagNo = 1;
            public static int Title = 2;
            public static int Description = 3;
            public static int DueDate = 4;
            public static int OverDue = 5;
            public static int Closed = 6;
            public static int Last = Closed;
        }

        public static class HistorySheetColumns
        {
            public static int TagNo = 1;
            public static int Description = 2;
            public static int DueInWeeks = 3;
            public static int Date = 4;
            public static int User = 5;
            public static int Details = 6;
            public static int Comment = 7;
            public static int Last = Comment;
        }

        private readonly ILogger<ExcelConverter> _logger;
        private Domain.Time.Timer _timer;

        public ExcelConverter(ILogger<ExcelConverter> logger) => _logger = logger;

        public MemoryStream Convert(ExportDto dto)
        {
            _timer = new Domain.Time.Timer();
            _logger.LogInformation("ExcelConverter start");
            // see https://github.com/ClosedXML/ClosedXML for sample code
            var excelStream = new MemoryStream();

            using (var workbook = new XLWorkbook())
            {
                _logger.LogInformation($"ExcelConverter CreateFrontSheet. {_timer.Elapsed()}");
                CreateFrontSheet(workbook, dto.UsedFilter);
                var exportTagDtos = dto.Tags.ToList();
                
                _logger.LogInformation($"ExcelConverter CreateTagSheet. {_timer.Elapsed()}");
                CreateTagSheet(workbook, exportTagDtos);
                
                _logger.LogInformation($"ExcelConverter CreateActionSheet. {_timer.Elapsed()}");
                CreateActionSheet(workbook, exportTagDtos);
                
                _logger.LogInformation($"ExcelConverter CreateHistorySheet. {_timer.Elapsed()}");
                CreateHistorySheet(workbook, exportTagDtos);

                _logger.LogInformation($"ExcelConverter saving. {_timer.Elapsed()}");
                workbook.SaveAs(excelStream);
            }

            _logger.LogInformation($"ExcelConverter saved and returning. {_timer.Elapsed()}");
            return excelStream;
        }

        private void CreateHistorySheet(XLWorkbook workbook, IList<ExportTagDto> tags)
        {
            if (tags.Count != 1)
            {
                return;
            }

            var sheet = workbook.Worksheets.Add("History");

            var rowIdx = 0;
            var row = sheet.Row(++rowIdx);
            row.Style.Font.SetBold();
            row.Style.Font.SetFontSize(12);
            row.Cell(HistorySheetColumns.TagNo).Value = "Tag nr";
            row.Cell(HistorySheetColumns.Description).Value = "Description";
            row.Cell(HistorySheetColumns.DueInWeeks).Value = "Due (weeks)";
            row.Cell(HistorySheetColumns.Date).Value = "Date (UTC)";
            row.Cell(HistorySheetColumns.User).Value = "User";
            row.Cell(HistorySheetColumns.Details).Value = "Preservation details";
            row.Cell(HistorySheetColumns.Comment).Value = "Preservation comment";

            var tag = tags.Single();
            foreach (var history in tag.History)
            {
                row = sheet.Row(++rowIdx);
            
                row.Cell(HistorySheetColumns.TagNo).SetValue(tag.TagNo);
                row.Cell(HistorySheetColumns.Description).SetValue(history.Description);
                row.Cell(HistorySheetColumns.DueInWeeks).SetValue(history.DueInWeeks);
                AddDateCell(row, HistorySheetColumns.Date, history.CreatedAtUtc);
                row.Cell(HistorySheetColumns.User).SetValue(history.CreatedBy);
                row.Cell(HistorySheetColumns.Details).SetValue(history.PreservationDetails);
                row.Cell(HistorySheetColumns.Comment).SetValue(history.PreservationComment);
            }
       
            const int minWidth = 10;
            const int maxWidth = 100;
            sheet.Columns(1, HistorySheetColumns.Last).AdjustToContents(1, rowIdx, minWidth, maxWidth);
        }

        private void CreateActionSheet(XLWorkbook workbook, IEnumerable<ExportTagDto> tags)
        {
            var sheet = workbook.Worksheets.Add("Actions");

            var rowIdx = 0;
            var row = sheet.Row(++rowIdx);
            row.Style.Font.SetBold();
            row.Style.Font.SetFontSize(12);
            row.Cell(ActionSheetColumns.TagNo).Value = "Tag nr";
            row.Cell(ActionSheetColumns.Title).Value = "Title";
            row.Cell(ActionSheetColumns.Description).Value = "Description";
            row.Cell(ActionSheetColumns.DueDate).Value = "Due date (UTC)";
            row.Cell(ActionSheetColumns.OverDue).Value = "Overdue";
            row.Cell(ActionSheetColumns.Closed).Value = "Closed (UTC)";

            foreach (var tag in tags.Where(t => t.Actions.Count > 0))
            {
                foreach (var action in tag.Actions)
                {
                    row = sheet.Row(++rowIdx);

                    row.Cell(ActionSheetColumns.TagNo).SetValue(tag.TagNo);
                    row.Cell(ActionSheetColumns.Title).SetValue(action.Title);
                    row.Cell(ActionSheetColumns.Description).SetValue(action.Description);
                    if (action.DueTimeUtc.HasValue)
                    {
                        AddDateCell(row, ActionSheetColumns.DueDate, action.DueTimeUtc.Value.Date);
                    }
                    row.Cell(ActionSheetColumns.OverDue).SetValue(action.IsOverDue);
                    if (action.ClosedAtUtc.HasValue)
                    {
                        AddDateCell(row, ActionSheetColumns.Closed, action.ClosedAtUtc.Value.Date);
                    }
                }
            }

            const int minWidth = 10;
            const int maxWidth = 100;
            sheet.Columns(1, ActionSheetColumns.Last).AdjustToContents(1, rowIdx, minWidth, maxWidth);
        }

        private void AddDateCell(IXLRow row, int cellIdx, DateTime date)
        {
            var cell = row.Cell(cellIdx);
            cell.SetValue(date);
            cell.Style.DateFormat.Format = "yyyy-mm-dd";
        }

        private void CreateTagSheet(XLWorkbook workbook, IEnumerable<ExportTagDto> tags)
        {
            var sheet = workbook.Worksheets.Add("Tags");

            var rowIdx = 0;
            var row = sheet.Row(++rowIdx);
            row.Style.Font.SetBold();
            row.Style.Font.SetFontSize(12);
            row.Cell(TagSheetColumns.TagNo).Value = "Tag nr";
            row.Cell(TagSheetColumns.Description).Value = "Tag description";
            row.Cell(TagSheetColumns.RequirementTitle).Value = "Requirement";
            row.Cell(TagSheetColumns.RequirementComment).Value = "Active requirement comment";
            row.Cell(TagSheetColumns.RequirementNextInYearAndWeek).Value = "Next preservation";
            row.Cell(TagSheetColumns.RequirementNextDueWeeks).Value = "Due (weeks)";
            row.Cell(TagSheetColumns.Journey).Value = "Journey";
            row.Cell(TagSheetColumns.Step).Value = "Step";
            row.Cell(TagSheetColumns.Mode).Value = "Mode";
            row.Cell(TagSheetColumns.Po).Value = "Purchase order";
            row.Cell(TagSheetColumns.Area).Value = "Area";
            row.Cell(TagSheetColumns.Resp).Value = "Responsible";
            row.Cell(TagSheetColumns.Disc).Value = "Discipline";
            row.Cell(TagSheetColumns.PresStatus).Value = "Status";
            row.Cell(TagSheetColumns.Remark).Value = "Remark";
            row.Cell(TagSheetColumns.StorageArea).Value = "Storage area";
            row.Cell(TagSheetColumns.CommPkg).Value = "Comm pkg";
            row.Cell(TagSheetColumns.McPkg).Value = "MC pkg";
            row.Cell(TagSheetColumns.ActionStatus).Value = "Action status";
            row.Cell(TagSheetColumns.Actions).Value = "Actions";
            row.Cell(TagSheetColumns.OpenActions).Value = "Open actions";
            row.Cell(TagSheetColumns.OverdueActions).Value = "Overdue actions";
            row.Cell(TagSheetColumns.Attachments).Value = "Attachments";
            row.Cell(TagSheetColumns.Voided).Value = "Is voided";

            foreach (var tag in tags)
            {
                foreach (var req in tag.Requirements)
                {
                    row = sheet.Row(++rowIdx);

                    row.Cell(TagSheetColumns.TagNo).SetValue(tag.TagNo);
                    row.Cell(TagSheetColumns.Description).SetValue(tag.Description);
                    row.Cell(TagSheetColumns.RequirementTitle).SetValue(req.RequirementTitle);
                    row.Cell(TagSheetColumns.RequirementComment).SetValue(req.ActiveComment);
                    row.Cell(TagSheetColumns.RequirementNextInYearAndWeek).SetValue(req.NextDueAsYearAndWeek);
                    if (req.NextDueWeeks.HasValue)
                    {
                        // The only number cell: NextDueWeeks
                        row.Cell(TagSheetColumns.RequirementNextDueWeeks).SetValue(req.NextDueWeeks.Value);
                    }
                    row.Cell(TagSheetColumns.Journey).SetValue(tag.Journey);
                    row.Cell(TagSheetColumns.Step).SetValue(tag.Step);
                    row.Cell(TagSheetColumns.Mode).SetValue(tag.Mode);
                    row.Cell(TagSheetColumns.Po).SetValue(tag.PurchaseOrderTitle);
                    row.Cell(TagSheetColumns.Area).SetValue(tag.AreaCode);
                    row.Cell(TagSheetColumns.Resp).SetValue(tag.ResponsibleCode);
                    row.Cell(TagSheetColumns.Disc).SetValue(tag.DisciplineCode);
                    row.Cell(TagSheetColumns.PresStatus).SetValue(tag.Status);
                    row.Cell(TagSheetColumns.Remark).SetValue(tag.Remark);
                    row.Cell(TagSheetColumns.StorageArea).SetValue(tag.StorageArea);
                    row.Cell(TagSheetColumns.CommPkg).SetValue(tag.CommPkgNo);
                    row.Cell(TagSheetColumns.McPkg).SetValue(tag.McPkgNo);
                    row.Cell(TagSheetColumns.ActionStatus).SetValue(tag.ActionStatus);
                    row.Cell(TagSheetColumns.Actions).SetValue(tag.ActionsCount);
                    row.Cell(TagSheetColumns.OpenActions).SetValue(tag.OpenActionsCount);
                    row.Cell(TagSheetColumns.OverdueActions).SetValue(tag.OverdueActionsCount);
                    row.Cell(TagSheetColumns.Attachments).SetValue(tag.AttachmentsCount);
                    row.Cell(TagSheetColumns.Voided).SetValue(tag.IsVoided);
                }
            }

            const int minWidth = 10;
            const int maxWidth = 100;
            sheet.Columns(1, TagSheetColumns.Last).AdjustToContents(1, rowIdx, minWidth, maxWidth);
        }

        private void CreateFrontSheet(XLWorkbook workbook, UsedFilterDto usedFilter)
        {
            var sheet = workbook.Worksheets.Add("Filters");
            var row = sheet.Row(FrontSheetRows.MainHeading);
            row.Style.Font.SetBold();
            row.Style.Font.SetFontSize(14);
            row.Cell(1).Value = "Export of preserved tags";

            AddUsedFilter(sheet.Row(FrontSheetRows.Plant), "Plant", usedFilter.Plant, true);
            AddUsedFilter(sheet.Row(FrontSheetRows.ProjectName), "Project", usedFilter.ProjectName, true);
            AddUsedFilter(sheet.Row(FrontSheetRows.ProjectDesc), "Project description", usedFilter.ProjectDescription, true);

            AddUsedFilter(sheet.Row(FrontSheetRows.FilterHeading), "Filter values:", "", true);

            AddUsedFilter(sheet.Row(FrontSheetRows.Tag), "Tag number starts with", usedFilter.TagNoStartsWith);
            AddUsedFilter(sheet.Row(FrontSheetRows.PO), "Purchase order number starts with", usedFilter.PurchaseOrderNoStartsWith);
            AddUsedFilter(sheet.Row(FrontSheetRows.CO), "Calloff number starts with", usedFilter.CallOffStartsWith);
            AddUsedFilter(sheet.Row(FrontSheetRows.CommPkg), "CommPkg number starts with", usedFilter.CommPkgNoStartsWith);
            AddUsedFilter(sheet.Row(FrontSheetRows.McPkg), "McPkg number starts with", usedFilter.McPkgNoStartsWith);
            AddUsedFilter(sheet.Row(FrontSheetRows.StorageArea), "Storage area starts with", usedFilter.StorageAreaStartsWith);
            AddUsedFilter(sheet.Row(FrontSheetRows.Status), "Preservation status", usedFilter.PreservationStatus);
            AddUsedFilter(sheet.Row(FrontSheetRows.Actions), "Preservation actions", usedFilter.ActionStatus);
            AddUsedFilter(sheet.Row(FrontSheetRows.Voided), "Voided/unvoided tags", usedFilter.VoidedFilter);
            AddUsedFilter(sheet.Row(FrontSheetRows.Due), "Preservation due date", usedFilter.DueFilters);
            AddUsedFilter(sheet.Row(FrontSheetRows.Journeys), "Journeys", usedFilter.JourneyTitles);
            AddUsedFilter(sheet.Row(FrontSheetRows.Modes), "Modes", usedFilter.ModeTitles);
            AddUsedFilter(sheet.Row(FrontSheetRows.Reqs), "Requirements", usedFilter.RequirementTypeTitles);
            AddUsedFilter(sheet.Row(FrontSheetRows.TF), "Tag functions", usedFilter.TagFunctionCodes);
            AddUsedFilter(sheet.Row(FrontSheetRows.Disc), "Disciplines", usedFilter.DisciplineCodes);
            AddUsedFilter(sheet.Row(FrontSheetRows.Resp), "Responsibles", usedFilter.ResponsibleCodes);
            AddUsedFilter(sheet.Row(FrontSheetRows.Areas), "Areas", usedFilter.AreaCodes);
         
            sheet.Columns(1, 2).AdjustToContents();
        }

        private void AddUsedFilter(IXLRow row, string label, IEnumerable<string> values)
            => AddUsedFilter(row, label, string.Join(",", values));

        private void AddUsedFilter(IXLRow row, string label, string value, bool bold = false)
        {
            row.Cell(1).SetValue(label);
            row.Cell(2).SetValue(value);
            row.Style.Font.SetBold(bold);
        }

        public string GetFileName()=> $"PreservedTags-{DateTime.Now:yyyyMMdd-hhmmss}";
    }
}
