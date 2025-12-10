using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTagsForExport;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagsQueries.GetTagsForExport
{
    [TestClass]
    public class GetTagsForExportQueryHandlerTests : ReadOnlyTestsBase
    {
        private GetTagsForExportQuery _query;
        private TestDataSet _testDataSet;
        private Mock<IOptionsSnapshot<TagOptions>> _apiOptionsMock;
        private Mock<ILogger<GetTagsForExportQueryHandler>> _loggerMock;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            _loggerMock = new Mock<ILogger<GetTagsForExportQueryHandler>>();
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                _query = new GetTagsForExportQuery(_testDataSet.Project1.Name);
            }

            _apiOptionsMock = new Mock<IOptionsSnapshot<TagOptions>>();
            _apiOptionsMock
                .Setup(x => x.Value)
                .Returns(new TagOptions { MaxHistoryExport = _testDataSet.Project1.Tags.Count });
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldReturnCorrectCounts()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var result = await dut.Handle(_query, default);
                // 30 tags added in setup, but 20 of them in project PX
                Assert.AreEqual(20, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldReturnCorrectDto()
        {
            Tag tag;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                tag = context.Tags.First();
                tag.AddAction(new Action(TestPlant, "A1", "Desc", null));
                tag.AddAction(new Action(TestPlant, "A2", "Desc", new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
                tag.AddAttachment(new TagAttachment(TestPlant, Guid.Empty, "F.txt"));
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.Single(t => t.TagNo == tag.TagNo);
                Assert.AreEqual(2, tagDto.ActionsCount);
                Assert.AreEqual(2, tagDto.OpenActionsCount);
                Assert.AreEqual(1, tagDto.OverdueActionsCount);
                Assert.AreEqual(1, tagDto.AttachmentsCount);

                AssertTag(tag, tagDto);
                AssertUsedFilter(result.Data.UsedFilter);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldNotReturnDueInfo_WhenPreservationNotStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.NotStarted.GetDisplayValue());

                foreach (var req in tagDto.Requirements)
                {
                    Assert.IsFalse(req.NextDueWeeks.HasValue);
                    Assert.IsNull(req.NextDueAsYearAndWeek);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldReturnDueInfo_WhenPreservationStarted()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active.GetDisplayValue());

                foreach (var req in tagDto.Requirements)
                {
                    Assert.AreEqual(_testDataSet.IntervalWeeks, req.NextDueWeeks);
                    Assert.IsNotNull(req.NextDueAsYearAndWeek);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldReturnNoElements_WhenThereIsNoTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery("NO"), default);
                Assert.AreEqual(0, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnTagNo()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var tagNoStartsWith = $"{_testDataSet.StdTagPrefix}-0";
                var filter = new Filter { TagNoStartsWith = tagNoStartsWith };

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 1);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.TagNo.StartsWith(tagNoStartsWith));
                }

                Assert.AreEqual(tagNoStartsWith, result.Data.UsedFilter.TagNoStartsWith);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldGetHistoryForManyTags_WhenLessThenMax()
        {
            var query = new GetTagsForExportQuery(_testDataSet.Project1.Name, HistoryExportMode.ExportMax);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var result = await dut.Handle(query, default);
                Assert.IsTrue(result.Data.Tags.Count > 1);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.History.Count > 0);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldNotGetHistoryForManyTags_WhenExceedMax()
        {
            var query = new GetTagsForExportQuery(_testDataSet.Project1.Name, HistoryExportMode.ExportMax);

            _apiOptionsMock
                .Setup(x => x.Value)
                .Returns(new TagOptions { MaxHistoryExport = _testDataSet.Project1.Tags.Count - 1 });

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var result = await dut.Handle(query, default);
                Assert.IsTrue(result.Data.Tags.Count > 1);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.History.Count == 0);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldGetHistoryForSingleTag()
        {
            var tagNoStartsWith = $"{_testDataSet.StdTagPrefix}-0";
            var comment = "Comment";
            var labelForNumberFirst = "Label for Number - first";
            var labelForNumberSecond = "Label for Number - second";
            var labelForNumberThird = "Label for Number - third";
            var labelForCheckBoxFirst = "Label for CheckBox - first";
            var labelForCheckBoxSecond = "Label for CheckBox - second";
            var labelForAttFirst = "Label for Attachment - first";
            var labelForAttSecond = "Label for Attachment - second";
            var labelForInfo = "Label for Info";
            History history;
            var number = 1282.91;
            var fileName = "filename.txt";
            Person currentUser;

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                currentUser = context.Persons.Single(p => p.Guid == _currentUserOid);
                var tag = context.Tags
                    .Include(t => t.Requirements)
                    .ThenInclude(r => r.PreservationPeriods)
                    .Single(t => t.TagNo == tagNoStartsWith);
                var reqDef = new RequirementDefinition(TestPlant, "Title", 2, RequirementUsage.ForAll, 1);
                var numberField1 = new Field(TestPlant, labelForNumberFirst, FieldType.Number, 1, "U", false);
                var numberField2 = new Field(TestPlant, labelForNumberSecond, FieldType.Number, 2, "U", false);
                var cbField1 = new Field(TestPlant, labelForCheckBoxFirst, FieldType.CheckBox, 3);
                var attField1 = new Field(TestPlant, labelForAttFirst, FieldType.Attachment, 4);
                var infoField = new Field(TestPlant, labelForInfo, FieldType.Info, 5);
                reqDef.AddField(numberField1);
                reqDef.AddField(numberField2);
                reqDef.AddField(cbField1);
                reqDef.AddField(attField1);
                reqDef.AddField(infoField);
                context.RequirementTypes.First().AddRequirementDefinition(reqDef);
                context.SaveChangesAsync().Wait();
                tag.AddRequirement(new TagRequirement(TestPlant, 4, reqDef));
                tag.StartPreservation();
                var tagRequirement = tag.Requirements.Last();
                tagRequirement.SetComment(comment);
                tagRequirement.RecordNumberValues(new Dictionary<int, double?>
                    {
                        {numberField1.Id, number},
                    },
                    reqDef);
                tagRequirement.RecordCheckBoxValues(new Dictionary<int, bool>
                    {
                        {cbField1.Id, true},
                    },
                    reqDef);
                tagRequirement.RecordAttachment(new FieldValueAttachment(TestPlant, Guid.NewGuid(), fileName),
                    attField1.Id,
                    reqDef);
                tagRequirement.RecordNumberIsNaValues(new List<int> { numberField2.Id }, reqDef);
                var activePeriodBeforePreservation = tagRequirement.ActivePeriod;
                tagRequirement.Preserve(_testDataSet.CurrentUser, false);
                history = new History(
                    TestPlant,
                    "Description",
                    tag.Guid,
                    ObjectType.Tag,
                    EventType.RequirementPreserved)
                {
                    DueInWeeks = 1,
                    PreservationRecordGuid = activePeriodBeforePreservation.PreservationRecord.Guid
                };
                context.History.Add(history);
                context.SaveChangesAsync().Wait();

                // also add other fields to requirement AFTER preservation done.
                // This is possible in real life, resulting that old preservation periods will exist without values for those fields
                var numberField3 = new Field(TestPlant, labelForNumberThird, FieldType.Number, 11, "U", false);
                var cbField2 = new Field(TestPlant, labelForCheckBoxSecond, FieldType.CheckBox, 12);
                var attField2 = new Field(TestPlant, labelForAttSecond, FieldType.Attachment, 13);
                reqDef.AddField(numberField3);
                reqDef.AddField(cbField2);
                reqDef.AddField(attField2);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var filter = new Filter { TagNoStartsWith = tagNoStartsWith };

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, HistoryExportMode.ExportMax, filter: filter), default);

                var tag = result.Data.Tags.Single();
                Assert.AreEqual(2, tag.History.Count);
                var historyDto = tag.History.Single(h => h.Id == history.Id);
                Assert.AreEqual(history.Description, historyDto.Description);
                Assert.AreEqual(history.DueInWeeks, historyDto.DueInWeeks);
                Assert.AreEqual(history.CreatedAtUtc, historyDto.CreatedAtUtc);
                Assert.AreEqual($"{currentUser.FirstName} {currentUser.LastName}", historyDto.CreatedBy);
                Assert.AreEqual(comment, historyDto.PreservationComment);
                Assert.IsTrue(historyDto.PreservationDetails.Contains($"{labelForNumberFirst}={number}."));
                Assert.IsTrue(historyDto.PreservationDetails.Contains($"{labelForNumberSecond}=N/A."));
                Assert.IsTrue(historyDto.PreservationDetails.Contains($"{labelForCheckBoxFirst}=true."));
                Assert.IsTrue(historyDto.PreservationDetails.Contains($"{labelForAttFirst}={fileName}."));
                Assert.IsFalse(historyDto.PreservationDetails.Contains(labelForInfo));
                Assert.IsTrue(historyDto.PreservationDetails.Contains($"{labelForNumberThird}=null."));
                Assert.IsTrue(historyDto.PreservationDetails.Contains($"{labelForCheckBoxSecond}=null."));
                Assert.IsTrue(historyDto.PreservationDetails.Contains($"{labelForAttSecond}=null."));
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnCommPkg()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var commPkgNoStartsWith = $"{_testDataSet.CommPkgPrefix}-0";
                var filter = new Filter { CommPkgNoStartsWith = commPkgNoStartsWith };

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(result.Data.Tags.Count(), 2);
                Assert.AreEqual(commPkgNoStartsWith, result.Data.UsedFilter.CommPkgNoStartsWith);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnMcPkg()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var mcPkgNoStartsWith = $"{_testDataSet.McPkgPrefix}-0";
                var filter = new Filter { McPkgNoStartsWith = mcPkgNoStartsWith };

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(2, result.Data.Tags.Count());
                Assert.AreEqual(mcPkgNoStartsWith, result.Data.UsedFilter.McPkgNoStartsWith);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnPurchaseOrder()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var purchaseOrderNoStartsWith = $"{_testDataSet.PoPrefix}-0";
                var filter = new Filter { PurchaseOrderNoStartsWith = purchaseOrderNoStartsWith };

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(2, tags.Count);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.PurchaseOrderTitle.StartsWith(purchaseOrderNoStartsWith));
                }
                Assert.AreEqual(purchaseOrderNoStartsWith, result.Data.UsedFilter.PurchaseOrderNoStartsWith);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnStorageArea()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var storageAreaStartsWith = $"{_testDataSet.StorageAreaPrefix}-0";
                var filter = new Filter { StorageAreaStartsWith = storageAreaStartsWith };

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(result.Data.Tags.Count(), 2);
                Assert.AreEqual(storageAreaStartsWith, result.Data.UsedFilter.StorageAreaStartsWith);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnCallOff()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var callOffStartsWith = $"{_testDataSet.CallOffPrefix}-0";
                var filter = new Filter { CallOffStartsWith = callOffStartsWith };

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 2);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.PurchaseOrderTitle.Contains(callOffStartsWith));
                }
                Assert.AreEqual(callOffStartsWith, result.Data.UsedFilter.CallOffStartsWith);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnPreservationStatus()
        {
            var filter = new Filter { PreservationStatus = new List<PreservationStatus>() { PreservationStatus.Active } };
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);

                Assert.AreEqual(0, result.Data.Tags.Count());
            }

            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 20);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(PreservationStatus.Active.GetDisplayValue(), tag.Status);
                }
                Assert.AreEqual(filter.PreservationStatus.Single().GetDisplayValue(), result.Data.UsedFilter.PreservationStatus.Single());
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnOpenActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                tag.AddAction(new Action(TestPlant, "A", "Desc", null));
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { ActionStatus = new List<ActionStatus> { ActionStatus.HasOpen } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 1);
                AssertActionStatus(result.Data.Tags, ActionStatus.HasOpen);
                Assert.AreEqual(ActionStatus.HasOpen.GetDisplayValue(), result.Data.UsedFilter.ActionStatus.Single());

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { ActionStatus = new List<ActionStatus> { ActionStatus.HasClosed } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(ActionStatus.HasClosed.GetDisplayValue(), result.Data.UsedFilter.ActionStatus.Single());

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { ActionStatus = new List<ActionStatus> { ActionStatus.HasOverdue } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(ActionStatus.HasOverdue.GetDisplayValue(), result.Data.UsedFilter.ActionStatus.Single());
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnClosedActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                var action = new Action(TestPlant, "A", "Desc", null);
                action.Close(_timeProvider.UtcNow, context.Persons.First());
                tag.AddAction(action);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { ActionStatus = new List<ActionStatus> { ActionStatus.HasOpen } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(ActionStatus.HasOpen.GetDisplayValue(), result.Data.UsedFilter.ActionStatus.Single());

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { ActionStatus = new List<ActionStatus> { ActionStatus.HasClosed } }), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 1);
                AssertActionStatus(tags, ActionStatus.HasClosed);
                Assert.AreEqual(ActionStatus.HasClosed.GetDisplayValue(), result.Data.UsedFilter.ActionStatus.Single());

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { ActionStatus = new List<ActionStatus> { ActionStatus.HasOverdue } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(ActionStatus.HasOverdue.GetDisplayValue(), result.Data.UsedFilter.ActionStatus.Single());
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnOverdueActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                var action = new Action(TestPlant, "A", "Desc", _timeProvider.UtcNow.AddDays(-1));
                tag.AddAction(action);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                // when filtering on tag which has Open actions, tags with overdue actions is included
                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { ActionStatus = new List<ActionStatus> { ActionStatus.HasOpen } }), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 1);
                AssertActionStatus(tags, ActionStatus.HasOverdue);
                Assert.AreEqual(ActionStatus.HasOpen.GetDisplayValue(), result.Data.UsedFilter.ActionStatus.Single());
                var tagWithOpenAndOverdueAction = tags.Single().TagNo;

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { ActionStatus = new List<ActionStatus> { ActionStatus.HasClosed } }), default);
                tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 0);
                Assert.AreEqual(ActionStatus.HasClosed.GetDisplayValue(), result.Data.UsedFilter.ActionStatus.Single());

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { ActionStatus = new List<ActionStatus> { ActionStatus.HasOverdue } }), default);
                tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 1);
                AssertActionStatus(tags, ActionStatus.HasOverdue);
                Assert.AreEqual(tagWithOpenAndOverdueAction, tags.Single().TagNo);
                Assert.AreEqual(ActionStatus.HasOverdue.GetDisplayValue(), result.Data.UsedFilter.ActionStatus.Single());
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldNotGetAnyTags_WhenFilterOnDue_WhenPreservationNotStarted()
        {
            var filter = new Filter { DueFilters = new List<DueFilterType> { DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek } };
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
            }

            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(3, result.Data.UsedFilter.DueFilters.Count());
                Assert.AreEqual(filter.DueFilters.ElementAt(0).GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));
                Assert.AreEqual(filter.DueFilters.ElementAt(1).GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(1));
                Assert.AreEqual(filter.DueFilters.ElementAt(2).GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldGetTagsDueNextWeek_WhenFilterOnDueNextWeek()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks - 1);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { DueFilters = new List<DueFilterType> { DueFilterType.NextWeek } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 20);
                Assert.AreEqual(DueFilterType.NextWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { DueFilters = new List<DueFilterType> { DueFilterType.ThisWeek } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.ThisWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { DueFilters = new List<DueFilterType> { DueFilterType.Overdue } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.Overdue.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek
                        }
                    }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 20);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldGetTagsDueThisWeek_WhenFilterOnDueThisWeek()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { DueFilters = new List<DueFilterType> { DueFilterType.NextWeek } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.NextWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { DueFilters = new List<DueFilterType> { DueFilterType.ThisWeek } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 20);
                Assert.AreEqual(DueFilterType.ThisWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { DueFilters = new List<DueFilterType> { DueFilterType.Overdue } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.Overdue.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek
                        }
                    }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 20);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldGetTagsOverdue_WhenFilterOnOverdue()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks + 1);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { DueFilters = new List<DueFilterType> { DueFilterType.NextWeek } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.NextWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { DueFilters = new List<DueFilterType> { DueFilterType.ThisWeek } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.ThisWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { DueFilters = new List<DueFilterType> { DueFilterType.Overdue } }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 20);
                Assert.AreEqual(DueFilterType.Overdue.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek
                        }
                    }), default);
                Assert.AreEqual(result.Data.Tags.Count(), 20);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnRequirementType()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter { RequirementTypeIds = new List<int> { _testDataSet.ReqType1.Id } };
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 10);
                var expectedRequirementTitle = _testDataSet.ReqType1.RequirementDefinitions.Single().Title;
                foreach (var tag in tags)
                {
                    Assert.AreEqual(expectedRequirementTitle, tag.Requirements.Single().RequirementTitle);
                }
                Assert.AreEqual(_testDataSet.ReqType1.Title, result.Data.UsedFilter.RequirementTypeTitles.ElementAt(0));
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnAreaCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var areaCode = $"{_testDataSet.AreaPrefix}-0";
                var filter = new Filter { AreaCodes = new List<string> { areaCode } };
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 2);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(areaCode, tag.AreaCode);
                }
                Assert.AreEqual(1, result.Data.UsedFilter.AreaCodes.Count());
                Assert.AreEqual(areaCode, result.Data.UsedFilter.AreaCodes.ElementAt(0));
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnDisciplineCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var diCode = $"{_testDataSet.DisciplinePrefix}-0";
                var filter = new Filter { DisciplineCodes = new List<string> { diCode } };
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 2);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(diCode, tag.DisciplineCode);
                }
                Assert.AreEqual(1, result.Data.UsedFilter.DisciplineCodes.Count());
                Assert.AreEqual(diCode, result.Data.UsedFilter.DisciplineCodes.ElementAt(0));
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnResponsible()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter { ResponsibleIds = new List<int> { _testDataSet.Responsible1.Id } };
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 20);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(_testDataSet.Responsible1.Code, tag.ResponsibleCode);
                }
                Assert.AreEqual(1, result.Data.UsedFilter.ResponsibleCodes.Count());
                Assert.AreEqual(_testDataSet.Responsible1.Code, result.Data.UsedFilter.ResponsibleCodes.ElementAt(0));
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnTagFunctionCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tfCode = $"{_testDataSet.TagFunctionPrefix}-0";
                var filter = new Filter { TagFunctionCodes = new List<string> { tfCode } };
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(result.Data.Tags.Count(), 2);
                Assert.AreEqual(1, result.Data.UsedFilter.TagFunctionCodes.Count());
                Assert.AreEqual(tfCode, result.Data.UsedFilter.TagFunctionCodes.ElementAt(0));
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnMode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter { ModeIds = new List<int> { _testDataSet.Mode1.Id } };
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 20);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(_testDataSet.Mode1.Title, tag.Mode);
                }
                Assert.AreEqual(1, result.Data.UsedFilter.ModeTitles.Count());
                Assert.AreEqual(_testDataSet.Mode1.Title, result.Data.UsedFilter.ModeTitles.ElementAt(0));
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnJourney()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter { JourneyIds = new List<int> { _testDataSet.Journey2With1Step.Id } };
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(result.Data.Tags.Count(), 10);
                Assert.AreEqual(1, result.Data.UsedFilter.JourneyTitles.Count());
                Assert.AreEqual(_testDataSet.Journey2With1Step.Title, result.Data.UsedFilter.JourneyTitles.ElementAt(0));
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnVoided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                tag.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { VoidedFilter = VoidedFilterType.NotVoided }), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 19);
                Assert.AreEqual(VoidedFilterType.NotVoided.GetDisplayValue(), result.Data.UsedFilter.VoidedFilter);
                AssertIsVoided(tags, false);

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter { VoidedFilter = VoidedFilterType.Voided }), default);
                tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 1);
                Assert.AreEqual(VoidedFilterType.Voided.GetDisplayValue(), result.Data.UsedFilter.VoidedFilter);
                AssertIsVoided(tags, true);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterWhenAllFiltersSet()
        {
            var filter = new Filter
            {
                PreservationStatus = new List<PreservationStatus>() { PreservationStatus.NotStarted },
                RequirementTypeIds = new List<int> { _testDataSet.ReqType1.Id },
                AreaCodes = new List<string> { $"{_testDataSet.AreaPrefix}-0" },
                DisciplineCodes = new List<string> { $"{_testDataSet.DisciplinePrefix}-0" },
                ResponsibleIds = new List<int> { _testDataSet.Responsible1.Id },
                TagFunctionCodes = new List<string> { $"{_testDataSet.TagFunctionPrefix}-0" },
                ModeIds = new List<int> { _testDataSet.Mode1.Id },
                JourneyIds = new List<int> { _testDataSet.Journey1With2Steps.Id },
                TagNoStartsWith = $"{_testDataSet.StdTagPrefix}-0",
                CommPkgNoStartsWith = $"{_testDataSet.CommPkgPrefix}-0",
                McPkgNoStartsWith = $"{_testDataSet.McPkgPrefix}-0",
                PurchaseOrderNoStartsWith = $"{_testDataSet.PoPrefix}-0",
                CallOffStartsWith = $"{_testDataSet.CallOffPrefix}-0",
                VoidedFilter = VoidedFilterType.NotVoided
            };
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(result.Data.Tags.Count(), 1);
                Assert.AreEqual(filter.VoidedFilter.GetDisplayValue(), result.Data.UsedFilter.VoidedFilter);
                Assert.AreEqual(filter.PreservationStatus.Single().GetDisplayValue(), result.Data.UsedFilter.PreservationStatus.Single());
                Assert.AreEqual(_testDataSet.ReqType1.Title, result.Data.UsedFilter.RequirementTypeTitles.ElementAt(0));
                Assert.AreEqual(filter.AreaCodes.ElementAt(0), result.Data.UsedFilter.AreaCodes.ElementAt(0));
                Assert.AreEqual(filter.DisciplineCodes.ElementAt(0), result.Data.UsedFilter.DisciplineCodes.ElementAt(0));
                Assert.AreEqual(_testDataSet.Responsible1.Code, result.Data.UsedFilter.ResponsibleCodes.ElementAt(0));
                Assert.AreEqual(filter.TagFunctionCodes.ElementAt(0), result.Data.UsedFilter.TagFunctionCodes.ElementAt(0));
                Assert.AreEqual(_testDataSet.Mode1.Title, result.Data.UsedFilter.ModeTitles.ElementAt(0));
                Assert.AreEqual(_testDataSet.Journey1With2Steps.Title, result.Data.UsedFilter.JourneyTitles.ElementAt(0));
                Assert.AreEqual(filter.TagNoStartsWith, result.Data.UsedFilter.TagNoStartsWith);
                Assert.AreEqual(filter.CommPkgNoStartsWith, result.Data.UsedFilter.CommPkgNoStartsWith);
                Assert.AreEqual(filter.McPkgNoStartsWith, result.Data.UsedFilter.McPkgNoStartsWith);
                Assert.AreEqual(filter.PurchaseOrderNoStartsWith, result.Data.UsedFilter.PurchaseOrderNoStartsWith);
                Assert.AreEqual(filter.CallOffStartsWith, result.Data.UsedFilter.CallOffStartsWith);
                Assert.AreEqual(filter.VoidedFilter.GetDisplayValue(), result.Data.UsedFilter.VoidedFilter);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldSortOnTagNo()
        {
            // filter on specific journey. Will get 10 standard tags
            var filter = new Filter { JourneyIds = new List<int> { _testDataSet.Journey1With2Steps.Id } };

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var sorting = new Sorting(SortingDirection.Asc, SortingProperty.TagNo);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, sorting: sorting, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-0", tags.First().TagNo);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-9", tags.Last().TagNo);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _apiOptionsMock.Object, _plantProvider, _loggerMock.Object);
                var sorting = new Sorting(SortingDirection.Desc, SortingProperty.TagNo);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, sorting: sorting, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-9", tags.First().TagNo);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-0", tags.Last().TagNo);
            }
        }

        private void StartPreservationOnAllTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tags = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).ToList();
                tags.ForEach(t => t.StartPreservation());
                context.SaveChangesAsync().Wait();
            }
        }

        private void AssertActionStatus(IEnumerable<ExportTagDto> tags, ActionStatus? actionStatus)
        {
            foreach (var tag in tags)
            {
                Assert.AreEqual(actionStatus.GetDisplayValue(), tag.ActionStatus);
            }
        }

        private void AssertIsVoided(IEnumerable<ExportTagDto> tags, bool isVoided)
        {
            foreach (var tag in tags)
            {
                Assert.AreEqual(isVoided, tag.IsVoided);
            }
        }

        private void AssertTag(Tag tag, ExportTagDto tagDto)
        {
            Assert.AreEqual("Has overdue action(s)", tagDto.ActionStatus);
            Assert.AreEqual(tag.AreaCode, tagDto.AreaCode);
            Assert.AreEqual(tag.CommPkgNo, tagDto.CommPkgNo);
            Assert.AreEqual(tag.DisciplineCode, tagDto.DisciplineCode);
            Assert.AreEqual(tag.IsVoided, tagDto.IsVoided);
            Assert.AreEqual(_testDataSet.Journey1With2Steps.Title, tagDto.Journey);
            Assert.AreEqual(_testDataSet.Journey1With2Steps.Steps.ElementAt(0).Title, tagDto.Step);
            Assert.AreEqual(tag.McPkgNo, tagDto.McPkgNo);
            Assert.AreEqual(_testDataSet.Mode1.Title, tagDto.Mode);
            Assert.AreEqual(_testDataSet.Responsible1.Code, tagDto.ResponsibleCode);
            Assert.AreEqual(tag.Description, tagDto.Description);
            Assert.AreEqual($"{tag.PurchaseOrderNo}/{tag.Calloff}", tagDto.PurchaseOrderTitle);
            Assert.AreEqual(tag.Status.GetDisplayValue(), tagDto.Status);
            Assert.AreEqual(tag.TagNo, tagDto.TagNo);
            Assert.AreEqual(tag.Remark, tagDto.Remark);
            Assert.AreEqual(tag.StorageArea, tagDto.StorageArea);
            Assert.AreEqual(_testDataSet.ReqType1.RequirementDefinitions.First().Title, tagDto.Requirements.Single().RequirementTitle);
            Assert.AreEqual(2, tagDto.ActionsCount);
            Assert.AreEqual(2, tagDto.OpenActionsCount);
            Assert.AreEqual(1, tagDto.OverdueActionsCount);
            Assert.AreEqual(1, tagDto.AttachmentsCount);

            AssertActions(tag.Actions, tagDto.Actions);
        }

        private void AssertActions(IReadOnlyCollection<Action> tagActions, List<ExportActionDto> tagDtoActions)
        {
            Assert.AreEqual(tagActions.Count, tagDtoActions.Count);
            foreach (var tagAction in tagActions)
            {
                var tagDtoAction = tagDtoActions.Single(a => a.Id == tagAction.Id);
                Assert.AreEqual(tagAction.DueTimeUtc, tagDtoAction.DueTimeUtc);
                Assert.AreEqual(tagAction.ClosedAtUtc, tagDtoAction.ClosedAtUtc);
                Assert.AreEqual(tagAction.Description, tagDtoAction.Description);
                Assert.AreEqual(tagAction.Title, tagDtoAction.Title);
                Assert.AreEqual(tagAction.IsOverDue(), tagDtoAction.IsOverDue);
            }
        }

        private void AssertUsedFilter(UsedFilterDto usedFilterDto)
        {
            Assert.AreEqual(TestPlant, usedFilterDto.Plant);
            Assert.AreEqual(_query.ProjectName, usedFilterDto.ProjectName);
            if (_query.Filter.ActionStatus.Any())
            {
                Assert.AreEqual(_query.Filter.ActionStatus.Select(s => s.GetDisplayValue()), usedFilterDto.ActionStatus);
            }
            else
            {
                Assert.AreEqual(0, usedFilterDto.ActionStatus.Count());
            }
            Assert.AreEqual(0, usedFilterDto.AreaCodes.Count());
            Assert.IsNull(usedFilterDto.CallOffStartsWith);
            Assert.IsNull(usedFilterDto.CommPkgNoStartsWith);
            Assert.AreEqual(0, usedFilterDto.DisciplineCodes.Count());
            Assert.AreEqual(0, usedFilterDto.DueFilters.Count());
            Assert.AreEqual(0, usedFilterDto.JourneyTitles.Count());
            Assert.IsNull(usedFilterDto.McPkgNoStartsWith);
            Assert.AreEqual(0, usedFilterDto.ModeTitles.Count());
            if (_query.Filter.PreservationStatus.Any())
            {
                Assert.AreEqual(_query.Filter.PreservationStatus.Select(s => s.GetDisplayValue()), usedFilterDto.PreservationStatus);
            }
            else
            {
                Assert.AreEqual(0, usedFilterDto.PreservationStatus.Count());
            }
            Assert.IsNull(usedFilterDto.PurchaseOrderNoStartsWith);
            Assert.AreEqual(0, usedFilterDto.RequirementTypeTitles.Count());
            Assert.AreEqual(0, usedFilterDto.ResponsibleCodes.Count());
            Assert.IsNull(usedFilterDto.StorageAreaStartsWith);
            Assert.AreEqual(0, usedFilterDto.TagFunctionCodes.Count());
            Assert.IsNull(usedFilterDto.TagNoStartsWith);
            Assert.AreEqual(_query.Filter.VoidedFilter.GetDisplayValue(), usedFilterDto.VoidedFilter);
        }
    }
}
