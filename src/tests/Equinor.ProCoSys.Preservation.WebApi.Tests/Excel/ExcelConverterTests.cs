using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport;
using Equinor.ProCoSys.Preservation.WebApi.Excel;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Excel
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
        
        private ExcelConverter _dut;
        private UsedFilterDto _usedFilterDto;
        private ExportTagDto _exportTagDtoWithoutActionsAndHistory;
        private ExportTagDto _exportTagDtoWithOneActionAndOneHistoryItems;
        private ExportTagDto _exportTagDtoWithTwoActionsAndTwoHistoryItems;

        [TestInitialize]
        public void Setup()
        {
            _dut = new ExcelConverter(new Mock<ILogger<ExcelConverter>>().Object);
            _usedFilterDto = new UsedFilterDto(
                "presActions",
                new List<string>{"ac1", "ac2"}, 
                "callOffStartsWith",
                "commPkgNoStartsWith",
                new List<string>{"dc1", "dc2"},
                new List<string>{"df1", "df2"},
                new List<string>{"j1", "j2"},
                "mcPkgNoStartsWith",
                new List<string>{"m1", "m2"},
                "preservationStatus",
                "projectDescription",
                "plant",
                "projectName",
                "purchaseOrderNoStartsWith",
                new List<string>{"rt1", "rt2"},
                new List<string>{"r1", "r2"},
                "storageAreaStartsWith",
                new List<string>{"tf"},
                "tagNoStartsWith",
                "voidedFilter");
            
            _exportTagDtoWithoutActionsAndHistory = new ExportTagDto(
                new List<ExportActionDto>(), 
                new List<ExportRequirementDto>(),
                "actionStatus1",
                1,
                "areaCode1",
                2,
                "commPkgNo1",
                "disciplineCode1",
                true,
                "journey1",
                "mcPkgNo1",
                "mode1",
                5,
                6,
                "purchaseOrderTitle1",
                "remark1",
                "responsibleCode1",
                "status1",
                "step1",
                "storageArea1",
                "tagDescription1",
                "tagNo1"
            );

            var actionDtos1 = new List<ExportActionDto>
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

            var reqDtos1 = new List<ExportRequirementDto>
            {
                new ExportRequirementDto(1,"R1", new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc), 4, "Comm1")
            };
            _exportTagDtoWithTwoActionsAndTwoHistoryItems = new ExportTagDto(
                actionDtos1,
                reqDtos1,
                "actionStatus2",
                11,
                "areaCode2",
                12,
                "commPkgNo2",
                "disciplineCode2",
                false,
                "journey2",
                "mcPkgNo2",
                "mode2",
                15,
                16,
                "purchaseOrderTitle2",
                "remark2",
                "responsibleCode2",
                "status2",
                "step2",
                "storageArea2",
                "tagDescription2",
                "tagNo2"
            );
            _exportTagDtoWithTwoActionsAndTwoHistoryItems.History.Add(
                new ExportHistoryDto(1, "H1", new DateTime(2021, 2, 3, 4, 5, 6, DateTimeKind.Utc), "Espen Askeladd", null, null, null));
            _exportTagDtoWithTwoActionsAndTwoHistoryItems.History.Add(
                new ExportHistoryDto(2, "H2", new DateTime(2021, 4, 5, 14, 15, 16, DateTimeKind.Utc), "Espen Askeladd", 2, "Details2", "Comment2"));

            var actionDtos2 = new List<ExportActionDto>
            {
                new ExportActionDto(
                    3,
                    "C",
                    "CDesc",
                    false,
                    null,
                    null)
            };

            var reqDtos2 = new List<ExportRequirementDto>
            {
                new ExportRequirementDto(2,"R2", new DateTime(2019, 1, 2, 3, 4, 5, DateTimeKind.Utc), 1, "Comm2"),
                new ExportRequirementDto(3,"R3", new DateTime(2020, 2, 3, 4, 5, 6, DateTimeKind.Utc), 4, "Comm3")
            };

            _exportTagDtoWithOneActionAndOneHistoryItems = new ExportTagDto(
                actionDtos2,
                reqDtos2,
                "actionStatus3",
                110,
                "areaCode3",
                120,
                "commPkgNo3",
                "disciplineCode3",
                false,
                "journey3",
                "mcPkgNo3",
                "mode3",
                150,
                160,
                "purchaseOrderTitle3",
                "remark3",
                "responsibleCode3",
                "status3",
                "step3",
                "storageArea3",
                "tagDescription3",
                "tagNo3"
            );
            _exportTagDtoWithOneActionAndOneHistoryItems.History.Add(
                new ExportHistoryDto(3, "H3", new DateTime(2021, 4, 5, 14, 15, 16, DateTimeKind.Utc), "Espen Askeladd", 2, "Details3", "Comment3"));
        }

        [TestMethod]
        public void Convert_DtoWithNoTags_ShouldCreateExcelWith3Sheets()
        {
            // Arrange
            var exportDto = new ExportDto(
                new List<ExportTagDto>(),
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected3Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet), exportDto.UsedFilter);
            AssertSheetExists(workbook, _historySheet, false);
        }
        
        [TestMethod]
        public void Convert_DtoWithOneTag_ShouldCreateExcelWith4Sheets()
        {
            // Arrange
            var exportDto = new ExportDto(
                new List<ExportTagDto>
                {
                    _exportTagDtoWithoutActionsAndHistory
                },
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected4Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet), exportDto.UsedFilter);
            AssertSheetExists(workbook, _historySheet, true);
        }

        [TestMethod]
        public void Convert_DtoWithManyTags_ShouldCreateExcelWith3Sheets()
        {
            // Arrange
            var exportDto = new ExportDto(
                new List<ExportTagDto>
                {
                    _exportTagDtoWithoutActionsAndHistory,
                    _exportTagDtoWithOneActionAndOneHistoryItems
                },
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected3Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet), exportDto.UsedFilter);
            AssertSheetExists(workbook, _historySheet, false);
        }

        [TestMethod]
        public void Convert_DtoWithNoTags_ShouldCreateExcelWithEmptyTagSheet()
        {
            // Arrange
            var exportDto = new ExportDto(
                new List<ExportTagDto>(),
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected3Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet), exportDto.UsedFilter);
            AssertTagSheet(workbook.Worksheets.Worksheet(_tagsSheet), exportDto.Tags);
        }

        [TestMethod]
        public void Convert_DtoWithNoTags_ShouldCreateExcelWithEmptyActionSheet()
        {
            // Arrange
            var exportDto = new ExportDto(
                new List<ExportTagDto>(),
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected3Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet), exportDto.UsedFilter);
            AssertActionSheet(workbook.Worksheets.Worksheet(_actionsSheet), exportDto.Tags);
        }

        [TestMethod]
        public void Convert_DtoWithOneTagWithoutAction_ShouldCreateExcelWithEmptyActionSheet()
        {
            // Arrange
            var exportDto = new ExportDto(
                new List<ExportTagDto>
                {
                    _exportTagDtoWithoutActionsAndHistory
                },
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected4Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet), exportDto.UsedFilter);
            AssertActionSheet(workbook.Worksheets.Worksheet(_actionsSheet), exportDto.Tags);
        }
        
        [TestMethod]
        public void Convert_DtoWithOneTagWithActionsAndHistory_ShouldCreateExcelWithCorrectDataInAllSheets()
        {
            // Arrange
            var exportDto = new ExportDto(
                new List<ExportTagDto>
                {
                    _exportTagDtoWithTwoActionsAndTwoHistoryItems
                },
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected4Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet), exportDto.UsedFilter);
            AssertTagSheet(workbook.Worksheets.Worksheet(_tagsSheet), exportDto.Tags);
            AssertActionSheet(workbook.Worksheets.Worksheet(_actionsSheet), exportDto.Tags);
            AssertHistorySheet(workbook.Worksheets.Worksheet(_historySheet), exportDto.Tags.Single());
        }
        
        [TestMethod]
        public void Convert_DtoWithManyTagsWithActionsAndHistory_ShouldCreateExcelWithCorrectDataInAllSheets()
        {
            // Arrange
            var exportDto = new ExportDto(
                new List<ExportTagDto>
                {
                    _exportTagDtoWithTwoActionsAndTwoHistoryItems,
                    _exportTagDtoWithOneActionAndOneHistoryItems
                },
                _usedFilterDto);

            // Act
            var xlStream = _dut.Convert(exportDto);

            // Assert
            var workbook = AssertWorkbookFromStream(xlStream, _expected3Sheets);
            AssertFiltersSheet(workbook.Worksheets.Worksheet(_filtersSheet), exportDto.UsedFilter);
            AssertTagSheet(workbook.Worksheets.Worksheet(_tagsSheet), exportDto.Tags);
            AssertActionSheet(workbook.Worksheets.Worksheet(_actionsSheet), exportDto.Tags);
            AssertSheetExists(workbook, _historySheet, false);
        }

        private void AssertFiltersSheet(IXLWorksheet worksheet, UsedFilterDto expextedFilterDto)
        {
            Assert.IsNotNull(worksheet);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.MainHeading, 1, "Export of preserved tags");
            AssertBlankRow(worksheet, ExcelConverter.FrontSheetRows.Blank1);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Plant, 2, "Plant", expextedFilterDto.Plant);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.ProjectName, 2, "Project", expextedFilterDto.ProjectName);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.ProjectDesc, 2, "Project description", expextedFilterDto.ProjectDescription);
            AssertBlankRow(worksheet, ExcelConverter.FrontSheetRows.Blank2);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.FilterHeading, 1, "Filter values:");
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Tag, 2, "Tag number starts with", expextedFilterDto.TagNoStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.PO, 2, "Purchase order number starts with", expextedFilterDto.PurchaseOrderNoStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.CO, 2, "Calloff number starts with", expextedFilterDto.CallOffStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.CommPkg, 2, "CommPkg number starts with", expextedFilterDto.CommPkgNoStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.McPkg, 2, "McPkg number starts with", expextedFilterDto.McPkgNoStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.StorageArea, 2, "Storage area starts with", expextedFilterDto.StorageAreaStartsWith);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Status, 2, "Preservation status", expextedFilterDto.PreservationStatus);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Actions, 2, "Preservation actions", expextedFilterDto.ActionStatus);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Voided, 2, "Voided/unvoided tags", expextedFilterDto.VoidedFilter);
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Due, 2, "Preservation due date", string.Join(",", expextedFilterDto.DueFilters));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Journeys, 2, "Journeys", string.Join(",", expextedFilterDto.JourneyTitles));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Modes, 2, "Modes", string.Join(",", expextedFilterDto.ModeTitles));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Reqs, 2, "Requirements", string.Join(",", expextedFilterDto.RequirementTypeTitles));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.TF, 2, "Tag functions", string.Join(",", expextedFilterDto.TagFunctionCodes));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Disc, 2, "Disciplines", string.Join(",", expextedFilterDto.DisciplineCodes));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Resp, 2, "Responsibles", string.Join(",", expextedFilterDto.ResponsibleCodes));
            AssertRow(worksheet, ExcelConverter.FrontSheetRows.Areas, 2, "Areas", string.Join(",", expextedFilterDto.AreaCodes));
        }

        private void AssertTagSheet(IXLWorksheet worksheet, IList<ExportTagDto> expectedTagData)
        {
            Assert.IsNotNull(worksheet);
            AssertHeadingsInTagSheet(worksheet);

            var rowCount = expectedTagData.SelectMany(t => t.Requirements).Count();            
            Assert.AreEqual(rowCount + 1, worksheet.RowsUsed().Count());

            var rowIndex = 1; // start at 1 because Row(1) is the header
            foreach (var tag in expectedTagData)
            {
                foreach (var req in tag.Requirements)
                {
                    rowIndex++;

                    var row = worksheet.Row(rowIndex); // + 2 because Row(1) is the header
                    Assert.AreEqual(tag.TagNo, row.Cell(ExcelConverter.TagSheetColumns.TagNo).Value);
                    Assert.AreEqual(tag.Description, row.Cell(ExcelConverter.TagSheetColumns.Description).Value);
                    Assert.AreEqual(req.RequirementTitle, row.Cell(ExcelConverter.TagSheetColumns.RequirementTitle).Value);
                    Assert.AreEqual(req.ActiveComment, row.Cell(ExcelConverter.TagSheetColumns.RequirementComment).Value);
                    Assert.AreEqual(req.NextDueAsYearAndWeek, row.Cell(ExcelConverter.TagSheetColumns.RequirementNextInYearAndWeek).Value);
                    AssertInt(req.NextDueWeeks, row.Cell(ExcelConverter.TagSheetColumns.RequirementNextDueWeeks).Value);
                    Assert.AreEqual(tag.Journey, row.Cell(ExcelConverter.TagSheetColumns.Journey).Value);
                    Assert.AreEqual(tag.Step, row.Cell(ExcelConverter.TagSheetColumns.Step).Value);
                    Assert.AreEqual(tag.Mode, row.Cell(ExcelConverter.TagSheetColumns.Mode).Value);
                    Assert.AreEqual(tag.PurchaseOrderTitle, row.Cell(ExcelConverter.TagSheetColumns.Po).Value);
                    Assert.AreEqual(tag.AreaCode, row.Cell(ExcelConverter.TagSheetColumns.Area).Value);
                    Assert.AreEqual(tag.ResponsibleCode, row.Cell(ExcelConverter.TagSheetColumns.Resp).Value);
                    Assert.AreEqual(tag.DisciplineCode, row.Cell(ExcelConverter.TagSheetColumns.Disc).Value);
                    Assert.AreEqual(tag.Status, row.Cell(ExcelConverter.TagSheetColumns.PresStatus).Value);
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
        }

        private void AssertActionSheet(IXLWorksheet worksheet, IList<ExportTagDto> expectedTagData)
        {
            Assert.IsNotNull(worksheet);
            AssertHeadingsInActionsSheet(worksheet);

            var expectedData = expectedTagData.SelectMany(t => t.Actions).ToList();
            Assert.AreEqual(expectedData.Count + 1, worksheet.RowsUsed().Count());

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

        private void AssertHistorySheet(IXLWorksheet worksheet, ExportTagDto expectedTagData)
        {
            Assert.IsNotNull(worksheet);
            AssertHeadingsInHistorySheet(worksheet);

            var expectedData = expectedTagData.History;
            Assert.AreEqual(expectedData.Count + 1, worksheet.RowsUsed().Count());

            var rowIdx = 2; // Start at 2 because Row(1) is the header
            foreach (var historyDto in expectedData)
            {
                var row = worksheet.Row(rowIdx++);

                Assert.AreEqual(expectedTagData.TagNo, row.Cell(ExcelConverter.HistorySheetColumns.TagNo).Value);
                Assert.AreEqual(historyDto.Description, row.Cell(ExcelConverter.HistorySheetColumns.Description).Value);
                AssertDateTime(historyDto.CreatedAtUtc, row.Cell(ExcelConverter.HistorySheetColumns.Date));
                Assert.AreEqual(historyDto.CreatedBy, row.Cell(ExcelConverter.HistorySheetColumns.User).Value);
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
            string expectedText1,
            string expectedText2 = null)
        {
            var row = worksheet.Row(rowIdx);
            Assert.AreEqual(expectedCellsUsed, row.CellsUsed().Count());
            Assert.AreEqual(expectedText1, row.Cell(1).Value);
            if (expectedCellsUsed > 1)
            {
                Assert.AreEqual(expectedText2, row.Cell(2).Value);
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
            Assert.AreEqual("Requirement", row.Cell(ExcelConverter.TagSheetColumns.RequirementTitle).Value);
            Assert.AreEqual("Active requirement comment", row.Cell(ExcelConverter.TagSheetColumns.RequirementComment).Value);
            Assert.AreEqual("Next preservation", row.Cell(ExcelConverter.TagSheetColumns.RequirementNextInYearAndWeek).Value);
            Assert.AreEqual("Due (weeks)", row.Cell(ExcelConverter.TagSheetColumns.RequirementNextDueWeeks).Value);
            Assert.AreEqual("Journey", row.Cell(ExcelConverter.TagSheetColumns.Journey).Value);
            Assert.AreEqual("Step", row.Cell(ExcelConverter.TagSheetColumns.Step).Value);
            Assert.AreEqual("Mode", row.Cell(ExcelConverter.TagSheetColumns.Mode).Value);
            Assert.AreEqual("Purchase order", row.Cell(ExcelConverter.TagSheetColumns.Po).Value);
            Assert.AreEqual("Area", row.Cell(ExcelConverter.TagSheetColumns.Area).Value);
            Assert.AreEqual("Responsible", row.Cell(ExcelConverter.TagSheetColumns.Resp).Value);
            Assert.AreEqual("Discipline", row.Cell(ExcelConverter.TagSheetColumns.Disc).Value);
            Assert.AreEqual("Status", row.Cell(ExcelConverter.TagSheetColumns.PresStatus).Value);
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
