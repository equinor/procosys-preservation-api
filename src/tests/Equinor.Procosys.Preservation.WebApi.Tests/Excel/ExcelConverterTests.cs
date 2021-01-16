using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;
using Equinor.Procosys.Preservation.WebApi.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Excel
{
    [TestClass]
    public class ExcelConverterTests
    {
        private static readonly string _filtersSheet = "Filters";
        private static readonly string _tagsSheet = "Tags";
        private static readonly string _actionsSheet = "Actions";
        private static readonly string _historySheet = "History";

        private static readonly string[] _expected3Sheets = {_filtersSheet, _tagsSheet, _actionsSheet};
        private static readonly string[] _expected4Sheets = {_filtersSheet, _tagsSheet, _actionsSheet, _historySheet};

        private static readonly string _presActions = "actions";
        private static readonly List<string> _areaCodes = new List<string>{"AreaCode1", "AreaCode2"};
        private static readonly string _callOffStartsWith = "callOffStartsWith";
        private static readonly string _commPkgNoStartsWith = "commPkgNoStartsWith";
        private static readonly List<string> _disciplineCodes = new List<string>{"DiscCode1", "DiscCode2"};
        private static readonly List<string> _dueFilters = new List<string>{"DueFilter1", "DueFilter2"};
        private static readonly List<string> _journeyTitles = new List<string>{"J1", "J2"};
        private static readonly string _mcPkgNoStartsWith = "mcPkgNoStartsWith";
        private static readonly List<string> _modeTitles = new List<string>{"Mode1", "Mode2"};
        private static readonly string _preservationStatus = "preservationStatus";
        private static readonly string _projectDescription = "projectDescription";
        private static readonly string _plant = "plant";
        private static readonly string _projectName = "projectName";
        private static readonly string _purchaseOrderNoStartsWith = "purchaseOrderNoStartsWith";
        private static readonly List<string> _requirementTypeTitles = new List<string> {"ReqType1", "ReqType2"};
        private static readonly List<string> _responsibleCodes = new List<string>{"Resp1", "Resp2"};
        private static readonly string _storageAreaStartsWith = "storageAreaStartsWith";
        private static readonly List<string> _tagFunctionCodes = new List<string>{"TF1", "TF2"};
        private static readonly string _tagNoStartsWith = "tagNoStartsWith";
        private static readonly string _voidedFilter = "voidedFilter";

        private static readonly string _actionStatus = "action";
        private static readonly int _actionsCount = 2;
        private static readonly string _areaCode = "area";
        private static readonly int _attachmentsCount = 3;
        private static readonly string _commPkgNo = "commPkg";
        private static readonly string _disciplineCode = "disc";
        private static readonly bool _isVoided = true;
        private static readonly string _journey = "J";
        private static readonly string _mcPkgNo = "mcPkg";
        private static readonly string _mode = "mode";
        private static readonly string _nextDueAsYearAndWeek = "200112";
        private static readonly int? _nextDueWeeks = 4;
        private static readonly int _openActionsCount = 5;
        private static readonly int _overdueActionsCount = 6;
        private static readonly string _purchaseOrderTitle ="po";
        private static readonly string _remark = "rem";
        private static readonly string _requirementTitles = "r1,r2";
        private static readonly string _responsibleCode = "resp";
        private static readonly string _status = "stat";
        private static readonly string _step = "step";
        private static readonly string _storageArea = "sa";
        private static readonly string _tagDescription = "tDesc";
        private static readonly string _tagNo = "tagNo";
        
        private ExcelConverter _dut;
        private UsedFilterDto _usedFilterDto;
        private ExportTagDto _exportTagDtoWithoutActionsAndHistory;
        private ExportTagDto _exportTagDtoWithTwoActionsAndTwoHistoryItems;

        [TestInitialize]
        public void Setup()
        {
            _dut = new ExcelConverter();
            _usedFilterDto = new UsedFilterDto(
                _presActions,
                _areaCodes,
                _callOffStartsWith,
                _commPkgNoStartsWith,
                _disciplineCodes,
                _dueFilters,
                _journeyTitles,
                _mcPkgNoStartsWith,
                _modeTitles,
                _preservationStatus,
                _projectDescription,
                _plant,
                _projectName,
                _purchaseOrderNoStartsWith,
                _requirementTypeTitles,
                _responsibleCodes,
                _storageAreaStartsWith,
                _tagFunctionCodes,
                _tagNoStartsWith,
                _voidedFilter);
            
            _exportTagDtoWithoutActionsAndHistory = new ExportTagDto(
                new List<ExportActionDto>(), 
                _actionStatus,
                _actionsCount,
                _areaCode,
                _attachmentsCount,
                _commPkgNo,
                _disciplineCode,
                _isVoided,
                _journey,
                _mcPkgNo,
                _mode,
                _nextDueAsYearAndWeek,
                _nextDueWeeks,
                _openActionsCount,
                _overdueActionsCount,
                _purchaseOrderTitle,
                _remark,
                _requirementTitles,
                _responsibleCode,
                _status,
                _step,
                _storageArea,
                _tagDescription,
                _tagNo
            );

            var actionDtos = new List<ExportActionDto>
            {
                new ExportActionDto(
                    1,
                    "A",
                    "ADesc",
                    true,
                    new DateTime(1970, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                    null),
                new ExportActionDto(
                    2,
                    "B",
                    "BDesc",
                    false,
                    null,
                    new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc))
            };

            _exportTagDtoWithTwoActionsAndTwoHistoryItems = new ExportTagDto(
                actionDtos,
                _actionStatus,
                _actionsCount,
                _areaCode,
                _attachmentsCount,
                _commPkgNo,
                _disciplineCode,
                _isVoided,
                _journey,
                _mcPkgNo,
                _mode,
                _nextDueAsYearAndWeek,
                _nextDueWeeks,
                _openActionsCount,
                _overdueActionsCount,
                _purchaseOrderTitle,
                _remark,
                _requirementTitles,
                _responsibleCode,
                _status,
                _step,
                _storageArea,
                _tagDescription,
                _tagNo
            );

            _exportTagDtoWithTwoActionsAndTwoHistoryItems.History.Add(
                new ExportHistoryDto(1, "H1", new DateTime(2021, 2, 3, 4, 5, 6, DateTimeKind.Utc), null, null, null));
            _exportTagDtoWithTwoActionsAndTwoHistoryItems.History.Add(
                new ExportHistoryDto(1, "H2", new DateTime(2021, 4, 5, 14, 15, 16, DateTimeKind.Utc), 2, "Details", "Comment"));
        }

        [TestMethod]
        public void Convert_DtoWithNoTags_ShouldCreateExcelWith3Sheets()
        {
            // Arrange
            var zeroTagDtos = new List<ExportTagDto>();
            var exportDto = new ExportDto(
                zeroTagDtos,
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected3Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet));
            AssertHistorySheet(workbook, _historySheet, zeroTagDtos);
        }

        [TestMethod]
        public void Convert_DtoWithNoTags_ShouldCreateExcelWithEmptyTagSheet()
        {
            // Arrange
            var zeroTagDtos = new List<ExportTagDto>();
            var exportDto = new ExportDto(
                zeroTagDtos,
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected3Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet));
            AssertTagSheet(workbook.Worksheets.Worksheet(_tagsSheet), zeroTagDtos);
        }

        [TestMethod]
        public void Convert_DtoWithNoTags_ShouldCreateExcelWithEmptyActionSheet()
        {
            // Arrange
            var zeroTagDtos = new List<ExportTagDto>();
            var exportDto = new ExportDto(
                zeroTagDtos,
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected3Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet));
            AssertActionSheet(workbook.Worksheets.Worksheet(_actionsSheet), zeroTagDtos);
        }

        [TestMethod]
        public void Convert_DtoWithOneTag_ShouldCreateExcelWith4Sheets()
        {
            // Arrange
            var exportTagDtos = new List<ExportTagDto>
            {
                _exportTagDtoWithoutActionsAndHistory
            };
            var exportDto = new ExportDto(
                exportTagDtos,
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected4Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet));
        }

        [TestMethod]
        public void Convert_DtoWithOneTagWithoutAction_ShouldCreateExcelWithEmptyActionSheet()
        {
            // Arrange
            var exportTagDtos = new List<ExportTagDto>
            {
                _exportTagDtoWithoutActionsAndHistory
            };
            var exportDto = new ExportDto(
                exportTagDtos,
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected4Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet));
            AssertActionSheet(workbook.Worksheets.Worksheet(_actionsSheet), exportTagDtos);
        }

        [TestMethod]
        public void Convert_DtoWithOneTagWithoutHistory_ShouldCreateExcelWithEmptyHistory()
        {
            // Arrange
            var exportTagDtos = new List<ExportTagDto>
            {
                _exportTagDtoWithoutActionsAndHistory
            };
            var exportDto = new ExportDto(
                exportTagDtos,
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected4Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet));
            AssertHistorySheet(workbook, _historySheet, exportTagDtos);
        }
        
        [TestMethod]
        public void Convert_DtoWithOneTagWithActionsAndHistory_ShouldCreateExcelWithCorrectDataInAllSheets()
        {
            // Arrange
            var exportTagDtos = new List<ExportTagDto>
            {
                _exportTagDtoWithTwoActionsAndTwoHistoryItems
            };
            var exportDto = new ExportDto(
                exportTagDtos,
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected4Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet));
            AssertTagSheet(workbook.Worksheets.Worksheet(_tagsSheet), exportTagDtos);
            AssertActionSheet(workbook.Worksheets.Worksheet(_actionsSheet), exportTagDtos);
            AssertHistorySheet(workbook, _historySheet, exportTagDtos);
        }

        private void AssertFiltersSheet(IXLWorksheet worksheet)
        {
            Assert.IsNotNull(worksheet);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.MainHeading, 1, "Export of preserved tags");
            AssertBlankRow(worksheet, ExcelConverter.FrontSheetRows.Blank1);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Plant, 2, "Plant", _plant);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.ProjectName, 2, "Project", _projectName);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.ProjectDesc, 2, "Project description", _projectDescription);
            AssertBlankRow(worksheet, ExcelConverter.FrontSheetRows.Blank2);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.FilterHeading, 1, "Filter values:");
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Tag, 2, "Tag number starts with", _tagNoStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.PO, 2, "Purchase order number starts with", _purchaseOrderNoStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.CO, 2, "Calloff number starts with", _callOffStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.CommPkg, 2, "CommPkg number starts with", _commPkgNoStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.McPkg, 2, "McPkg number starts with", _mcPkgNoStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.StorageArea, 2, "Storage area starts with", _storageAreaStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Status, 2, "Preservation status", _preservationStatus);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Actions, 2, "Preservation actions", _presActions);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Voided, 2, "Voided/unvoided tags", _voidedFilter);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Due, 2, "Preservation due date", string.Join(",", _dueFilters));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Journeys, 2, "Journeys", string.Join(",", _journeyTitles));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Modes, 2, "Modes", string.Join(",", _modeTitles));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Reqs, 2, "Requirements", string.Join(",", _requirementTypeTitles));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.TF, 2, "Tag functions", string.Join(",", _tagFunctionCodes));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Disc, 2, "Disciplines", string.Join(",", _disciplineCodes));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Resp, 2, "Responsibles", string.Join(",", _responsibleCodes));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Areas, 2, "Areas", string.Join(",", _areaCodes));
        }

        private void AssertTagSheet(IXLWorksheet worksheet, List<ExportTagDto> expectedTagData)
        {
            Assert.IsNotNull(worksheet);
            AssertHeadingsInTagSheet(worksheet);

            if (expectedTagData.Count == 0)
            {
                Assert.AreEqual(1, worksheet.RowsUsed().Count());
                return;
            }

            Assert.AreEqual(expectedTagData.Count + 1, worksheet.RowsUsed().Count());

            for (var i = 0; i < expectedTagData.Count; i++)
            {
                var tag = expectedTagData.ElementAt(i);
                var row = worksheet.Row(i + 2); // + 2 because Row(1) is the header
                Assert.AreEqual(tag.TagNo, row.Cell(ExcelConverter.TagSheetColumns.TagNo).Value);
                Assert.AreEqual(tag.Description, row.Cell(ExcelConverter.TagSheetColumns.Description).Value);
                Assert.AreEqual(tag.NextDueAsYearAndWeek, row.Cell(ExcelConverter.TagSheetColumns.NextInYearAndWeek).Value);
                AssertInt(tag.NextDueWeeks, row.Cell(ExcelConverter.TagSheetColumns.NextDueWeeks).Value);
                Assert.AreEqual(tag.Journey, row.Cell(ExcelConverter.TagSheetColumns.Journey).Value);
                Assert.AreEqual(tag.Step, row.Cell(ExcelConverter.TagSheetColumns.Step).Value);
                Assert.AreEqual(tag.Mode, row.Cell(ExcelConverter.TagSheetColumns.Mode).Value);
                Assert.AreEqual(tag.PurchaseOrderTitle, row.Cell(ExcelConverter.TagSheetColumns.Po).Value);
                Assert.AreEqual(tag.AreaCode, row.Cell(ExcelConverter.TagSheetColumns.Area).Value);
                Assert.AreEqual(tag.ResponsibleCode, row.Cell(ExcelConverter.TagSheetColumns.Resp).Value);
                Assert.AreEqual(tag.DisciplineCode, row.Cell(ExcelConverter.TagSheetColumns.Disc).Value);
                Assert.AreEqual(tag.Status, row.Cell(ExcelConverter.TagSheetColumns.PresStatus).Value);
                Assert.AreEqual(tag.RequirementTitles, row.Cell(ExcelConverter.TagSheetColumns.Req).Value);
                Assert.AreEqual(tag.Remark, row.Cell(ExcelConverter.TagSheetColumns.Remark).Value);
                Assert.AreEqual(tag.StorageArea, row.Cell(ExcelConverter.TagSheetColumns.StorageArea).Value);
                Assert.AreEqual(tag.ActionStatus, row.Cell(ExcelConverter.TagSheetColumns.ActionStatus).Value);
                Assert.AreEqual(tag.CommPkgNo, row.Cell(ExcelConverter.TagSheetColumns.CommPkg).Value);
                Assert.AreEqual(tag.McPkgNo, row.Cell(ExcelConverter.TagSheetColumns.McPkg).Value);
                AssertInt(tag.ActionsCount, row.Cell(ExcelConverter.TagSheetColumns.Actions).Value);
                AssertInt(tag.OpenActionsCount, row.Cell(ExcelConverter.TagSheetColumns.OpenActions).Value);
                AssertInt(tag.OverdueActionsCount, row.Cell(ExcelConverter.TagSheetColumns.OverdueActions).Value);
                AssertInt(tag.AttachmentsCount, row.Cell(ExcelConverter.TagSheetColumns.Attachments).Value);
                AssertBool(tag.IsVoided, row.Cell(ExcelConverter.TagSheetColumns.Voided).Value);
            }
        }

        private void AssertActionSheet(IXLWorksheet worksheet, List<ExportTagDto> expectedTagData)
        {
            Assert.IsNotNull(worksheet);
            AssertHeadingsInActionsSheet(worksheet);

            var expectedData = expectedTagData.SelectMany(t => t.Actions).ToList();
            Assert.AreEqual(expectedData.Count + 1, worksheet.RowsUsed().Count());

            if (expectedData.Count == 0)
            {
                return;
            }

            var rowIdx = 2; // Start at 2 because Row(1) is the header
            foreach (var tag in expectedTagData.Where(t => t.Actions.Count > 0))
            {
                foreach (var action in tag.Actions)
                {
                    var row = worksheet.Row(rowIdx++);

                    Assert.AreEqual(tag.TagNo, row.Cell(ExcelConverter.ActionSheetColumns.TagNo).Value);
                    Assert.AreEqual(action.Title, row.Cell(ExcelConverter.ActionSheetColumns.Title).Value);
                    Assert.AreEqual(action.IsOverDue.ToString().ToUpper(),
                        row.Cell(ExcelConverter.ActionSheetColumns.OverDue).Value.ToString()?.ToUpper());
                    Assert.AreEqual(action.Description, row.Cell(ExcelConverter.ActionSheetColumns.Description).Value);
                    AssertDateTime(action.DueTimeUtc, row.Cell(ExcelConverter.ActionSheetColumns.DueDate));
                    AssertDateTime(action.ClosedAtUtc, row.Cell(ExcelConverter.ActionSheetColumns.Closed));
                }
            }
        }

        private void AssertHistorySheet(XLWorkbook workbook, string expectedSheet, List<ExportTagDto> expectedTagData)
        {
            var expectHistorySheetToExists = expectedTagData.Count == 1;
            AssertSheetExists(workbook, expectedSheet, expectHistorySheetToExists);

            if (!expectHistorySheetToExists)
            {
                return;
            }

            var worksheet = workbook.Worksheets.Worksheet(expectedSheet);
            Assert.IsNotNull(worksheet);
            AssertHeadingsInHistorySheet(worksheet);

            var tag = expectedTagData.Single();
            var expectedData = tag.History;
            Assert.AreEqual(expectedData.Count + 1, worksheet.RowsUsed().Count());
            
            if (expectedData.Count == 0)
            {
                return;
            }

            var rowIdx = 2; // Start at 2 because Row(1) is the header
            foreach (var historyDto in expectedData)
            {
                var row = worksheet.Row(rowIdx++);

                Assert.AreEqual(tag.TagNo, row.Cell(ExcelConverter.HistorySheetColumns.TagNo).Value);
                Assert.AreEqual(historyDto.Description, row.Cell(ExcelConverter.HistorySheetColumns.Description).Value);
                AssertDateTime(historyDto.CreatedAtUtc, row.Cell(ExcelConverter.HistorySheetColumns.Date));
                AssertInt(historyDto.DueInWeeks, row.Cell(ExcelConverter.HistorySheetColumns.DueInWeeks).Value);
            }
        }

        private void AssertBlankRow(IXLWorksheet worksheet, int rowIdx)
        {
            var row = worksheet.Row(rowIdx);
            Assert.AreEqual(0, row.CellsUsed().Count());
        }

        private static void AssertRow(
            IXLWorksheet worksheet,
            int rowIdx,
            int expectedCellsUsed,
            string expectedTekst1,
            string expectedTekst2 = null)
        {
            var row = worksheet.Row(rowIdx);
            Assert.AreEqual(expectedCellsUsed, row.CellsUsed().Count());
            Assert.AreEqual(expectedTekst1, row.Cell(1).Value);
            if (expectedCellsUsed > 1)
            {
                Assert.AreEqual(expectedTekst2, row.Cell(2).Value);
            }
        }

        private static XLWorkbook AssertWorkbookFromStream(MemoryStream xlStream, string[] expectedSheets)
        {
            var workbook = new XLWorkbook(xlStream);
            Assert.IsNotNull(workbook);
            Assert.IsNotNull(workbook.Worksheets);
            Assert.AreEqual(expectedSheets.Length, workbook.Worksheets.Count);
            foreach (var expectedSheet in expectedSheets)
            {
                AssertSheetExists(workbook, expectedSheet, true);
            }
            return workbook;
        }

        private static void AssertSheetExists(XLWorkbook workbook, string expectedSheet, bool shouldExists)
        {
            var sheetFound = workbook.Worksheets.TryGetWorksheet(expectedSheet, out _);
            Assert.AreEqual(shouldExists, sheetFound);
        }

        private static void AssertHeadingsInTagSheet(IXLWorksheet worksheet)
        {
            var row = worksheet.Row(1);

            Assert.AreEqual(ExcelConverter.TagSheetColumns.Last, row.CellsUsed().Count());
            Assert.AreEqual("Tag nr", row.Cell(ExcelConverter.TagSheetColumns.TagNo).Value);
            Assert.AreEqual("Tag description", row.Cell(ExcelConverter.TagSheetColumns.Description).Value);
            Assert.AreEqual("Next preservation", row.Cell(ExcelConverter.TagSheetColumns.NextInYearAndWeek).Value);
            Assert.AreEqual("Due (weeks)", row.Cell(ExcelConverter.TagSheetColumns.NextDueWeeks).Value);
            Assert.AreEqual("Journey", row.Cell(ExcelConverter.TagSheetColumns.Journey).Value);
            Assert.AreEqual("Step", row.Cell(ExcelConverter.TagSheetColumns.Step).Value);
            Assert.AreEqual("Mode", row.Cell(ExcelConverter.TagSheetColumns.Mode).Value);
            Assert.AreEqual("Purchase order", row.Cell(ExcelConverter.TagSheetColumns.Po).Value);
            Assert.AreEqual("Area", row.Cell(ExcelConverter.TagSheetColumns.Area).Value);
            Assert.AreEqual("Responsible", row.Cell(ExcelConverter.TagSheetColumns.Resp).Value);
            Assert.AreEqual("Discipline", row.Cell(ExcelConverter.TagSheetColumns.Disc).Value);
            Assert.AreEqual("Status", row.Cell(ExcelConverter.TagSheetColumns.PresStatus).Value);
            Assert.AreEqual("Requirements", row.Cell(ExcelConverter.TagSheetColumns.Req).Value);
            Assert.AreEqual("Remark", row.Cell(ExcelConverter.TagSheetColumns.Remark).Value);
            Assert.AreEqual("Storage area", row.Cell(ExcelConverter.TagSheetColumns.StorageArea).Value);
            Assert.AreEqual("Comm pkg", row.Cell(ExcelConverter.TagSheetColumns.CommPkg).Value);
            Assert.AreEqual("MC pkg", row.Cell(ExcelConverter.TagSheetColumns.McPkg).Value);
            Assert.AreEqual("Action status", row.Cell(ExcelConverter.TagSheetColumns.ActionStatus).Value);
            Assert.AreEqual("Actions", row.Cell(ExcelConverter.TagSheetColumns.Actions).Value);
            Assert.AreEqual("Open actions", row.Cell(ExcelConverter.TagSheetColumns.OpenActions).Value);
            Assert.AreEqual("Overdue actions", row.Cell(ExcelConverter.TagSheetColumns.OverdueActions).Value);
            Assert.AreEqual("Attachments", row.Cell(ExcelConverter.TagSheetColumns.Attachments).Value);
            Assert.AreEqual("Is voided", row.Cell(ExcelConverter.TagSheetColumns.Voided).Value);
        }

        private static void AssertHeadingsInActionsSheet(IXLWorksheet worksheet)
        {
            var row = worksheet.Row(1);

            Assert.AreEqual(ExcelConverter.ActionSheetColumns.Last, row.CellsUsed().Count());
            Assert.AreEqual("Tag nr", row.Cell(ExcelConverter.ActionSheetColumns.TagNo).Value);
            Assert.AreEqual("Title", row.Cell(ExcelConverter.ActionSheetColumns.Title).Value);
            Assert.AreEqual("Description", row.Cell(ExcelConverter.ActionSheetColumns.Description).Value);
            Assert.AreEqual("Overdue", row.Cell(ExcelConverter.ActionSheetColumns.OverDue).Value);
            Assert.AreEqual("Due date (UTC)", row.Cell(ExcelConverter.ActionSheetColumns.DueDate).Value);
            Assert.AreEqual("Closed (UTC)", row.Cell(ExcelConverter.ActionSheetColumns.Closed).Value);
        }

        private static void AssertHeadingsInHistorySheet(IXLWorksheet worksheet)
        {
            var row = worksheet.Row(1);

            Assert.AreEqual(ExcelConverter.HistorySheetColumns.Last, row.CellsUsed().Count());
            Assert.AreEqual("Tag nr", row.Cell(ExcelConverter.HistorySheetColumns.TagNo).Value);
            Assert.AreEqual("Description", row.Cell(ExcelConverter.HistorySheetColumns.Description).Value);
            Assert.AreEqual("Due (weeks)", row.Cell(ExcelConverter.HistorySheetColumns.DueInWeeks).Value);
            Assert.AreEqual("Date (UTC)", row.Cell(ExcelConverter.HistorySheetColumns.Date).Value);
            Assert.AreEqual("Preservation details", row.Cell(ExcelConverter.HistorySheetColumns.Details).Value);
            Assert.AreEqual("Preservation comment", row.Cell(ExcelConverter.HistorySheetColumns.Comment).Value);
        }
         
        private void AssertInt(int? expectedValue, object value)
        {
            if (expectedValue.HasValue)
            {
                Assert.AreEqual((double)expectedValue.Value, value);

            }
            else
            {
                Assert.AreEqual(string.Empty, value);
            }
        }

        private void AssertBool(bool b, object value)
            => Assert.AreEqual(b.ToString().ToUpper(), value?.ToString()?.ToUpper());

        private void AssertDateTime(DateTime? expectedUtcValue, IXLCell cell)
        {
            if (expectedUtcValue.HasValue)
            {
                Assert.IsInstanceOfType(cell.Value, typeof(DateTime));
                Assert.AreEqual("yyyy-mm-dd", cell.Style.DateFormat.Format);
                Assert.AreEqual(expectedUtcValue.Value.Date, ((DateTime)cell.Value).Date);
            }
            else
            {
                Assert.AreEqual(string.Empty, cell.Value);
            }
        }
    }
}
