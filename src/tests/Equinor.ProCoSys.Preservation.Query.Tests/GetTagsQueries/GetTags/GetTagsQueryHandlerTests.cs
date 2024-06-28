using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagsQueries.GetTags
{
    [TestClass]
    public class GetTagsQueryHandlerTests : ReadOnlyTestsBase
    {
        private GetTagsQuery _query;
        private int _tagIsNewHours = 12;
        private Mock<IOptionsSnapshot<TagOptions>> _apiOptionsMock;
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            _apiOptionsMock = new Mock<IOptionsSnapshot<TagOptions>>();
            _apiOptionsMock
                .Setup(x => x.Value)
                .Returns(new TagOptions { IsNewHours = _tagIsNewHours });

            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                _query = new GetTagsQuery(_testDataSet.Project1.Name);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnCorrectCounts()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);
                // 30 tags added in setup, but 20 of them in project PX
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnPageSize()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var paging = new Paging(0, 5);
                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, paging: paging), default);
                Assert.AreEqual(20, result.Data.MaxAvailable);
                Assert.AreEqual(5, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnEmptyPageButMaxAvailable_WhenGettingBehindLastPage()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var paging = new Paging(1, 50);
                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, paging: paging), default);
                Assert.AreEqual(20, result.Data.MaxAvailable);
                Assert.AreEqual(0, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnCorrectDto()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.TagNo.StartsWith(_testDataSet.StdTagPrefix));
                var tag = context.Tags.Single(t => t.Id == tagDto.Id);
                Assert.IsNull(tagDto.ActionStatus);
                Assert.AreEqual(tag.AreaCode, tagDto.AreaCode);
                Assert.AreEqual(tag.Calloff, tagDto.CalloffNo);
                Assert.AreEqual(tag.CommPkgNo, tagDto.CommPkgNo);
                Assert.AreEqual(tag.DisciplineCode, tagDto.DisciplineCode);
                Assert.AreEqual(tag.Id, tagDto.Id);
                Assert.AreEqual(tag.IsVoided, tagDto.IsVoided);
                Assert.AreEqual(tag.McPkgNo, tagDto.McPkgNo);
                Assert.AreEqual(_testDataSet.Mode1.Title, tagDto.Mode);
                Assert.AreEqual(_testDataSet.Responsible1.Code, tagDto.ResponsibleCode);
                Assert.AreEqual(_testDataSet.Mode2.Title, tagDto.NextMode);
                Assert.AreEqual(_testDataSet.Responsible2.Code, tagDto.NextResponsibleCode);
                Assert.AreEqual(tag.Description, tagDto.Description);
                Assert.AreEqual(tag.PurchaseOrderNo, tagDto.PurchaseOrderNo);
                Assert.AreEqual(tag.Status.GetDisplayValue(), tagDto.Status);
                Assert.AreEqual(tag.TagFunctionCode, tagDto.TagFunctionCode);
                Assert.AreEqual(tag.TagNo, tagDto.TagNo);
                Assert.AreEqual(_testDataSet.ReqType1.Code, tagDto.Requirements.First().RequirementTypeCode);
                Assert.AreEqual(_testDataSet.ReqType1.Icon, tagDto.Requirements.First().RequirementTypeIcon);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldNotReturnDueInfo_WhenPreservationNotStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.NotStarted.GetDisplayValue());
                var requirementDto = tagDto.Requirements.First();

                Assert.IsFalse(requirementDto.NextDueTimeUtc.HasValue);
                Assert.IsFalse(requirementDto.NextDueWeeks.HasValue);
                Assert.IsNull(requirementDto.NextDueAsYearAndWeek);
                Assert.AreEqual(PreservationStatus.NotStarted.GetDisplayValue(), tagDto.Status);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnDueInfo_WhenPreservationStarted()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active.GetDisplayValue());
                var requirementDto = tagDto.Requirements.First();

                Assert.IsTrue(requirementDto.NextDueTimeUtc.HasValue);
                Assert.AreEqual(_testDataSet.IntervalWeeks, requirementDto.NextDueWeeks);
                Assert.IsNotNull(requirementDto.NextDueAsYearAndWeek);
                Assert.AreEqual(PreservationStatus.Active.GetDisplayValue(), tagDto.Status);
            }
        }
        
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnCorrectStatuses_BeforePreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.NotStarted.GetDisplayValue() && t.TagType == TagType.Standard);

                Assert.IsTrue(stdTagActiveDto.ReadyToBeEdited);
                Assert.IsFalse(stdTagActiveDto.ReadyToBePreserved);
                Assert.IsFalse(stdTagActiveDto.ReadyToBeRescheduled);
                Assert.IsTrue(stdTagActiveDto.ReadyToBeStarted);
                Assert.IsTrue(stdTagActiveDto.ReadyToBeTransferred);
                Assert.IsFalse(stdTagActiveDto.ReadyToUndoStarted);

                var siteTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.NotStarted.GetDisplayValue() && t.TagType == TagType.SiteArea);
                Assert.IsTrue(siteTagActiveDto.ReadyToBeEdited);
                Assert.IsFalse(siteTagActiveDto.ReadyToBePreserved);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeRescheduled);
                Assert.IsTrue(siteTagActiveDto.ReadyToBeStarted);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeTransferred);
                Assert.IsFalse(siteTagActiveDto.ReadyToUndoStarted);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnCorrectStatuses_WhenPreservationStarted()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active.GetDisplayValue() && t.TagType == TagType.Standard);

                Assert.IsTrue(stdTagActiveDto.ReadyToBeEdited);
                Assert.IsTrue(stdTagActiveDto.ReadyToBeTransferred);
                Assert.IsTrue(stdTagActiveDto.ReadyToBeRescheduled);
                Assert.IsFalse(stdTagActiveDto.ReadyToBeStarted);
                Assert.IsTrue(stdTagActiveDto.ReadyToUndoStarted);

                var siteTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active.GetDisplayValue() && t.TagType == TagType.SiteArea);
                Assert.IsTrue(siteTagActiveDto.ReadyToBeEdited);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeTransferred);
                Assert.IsTrue(siteTagActiveDto.ReadyToBeRescheduled);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeStarted);
                Assert.IsTrue(siteTagActiveDto.ReadyToUndoStarted);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnCorrectStatuses_WhenPreservationCompleted()
        {
            StartPreservationOnAllTags();

            CompleteAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Completed.GetDisplayValue() && t.TagType == TagType.Standard);

                Assert.IsFalse(stdTagActiveDto.ReadyToBeEdited);
                Assert.IsFalse(stdTagActiveDto.ReadyToBeTransferred);
                Assert.IsFalse(stdTagActiveDto.ReadyToBeRescheduled);
                Assert.IsTrue(stdTagActiveDto.ReadyToBeStarted);
                Assert.IsFalse(stdTagActiveDto.ReadyToUndoStarted);

                var siteTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Completed.GetDisplayValue() && t.TagType == TagType.SiteArea);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeEdited);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeTransferred);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeRescheduled);
                Assert.IsTrue(siteTagActiveDto.ReadyToBeStarted);
                Assert.IsFalse(siteTagActiveDto.ReadyToUndoStarted);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldNotReturnReadyToBeTransferredForStandardTags_WhenTransferredToLastStep()
        {
            StartPreservationOnAllTags();
            
            TransferAllStandardTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active.GetDisplayValue() && t.TagType == TagType.Standard);

                Assert.IsFalse(stdTagActiveDto.ReadyToBeTransferred);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldNotReturnReadyToBePreserved_BeforeDue()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active.GetDisplayValue() && t.TagType == TagType.Standard);

                Assert.IsFalse(stdTagActiveDto.ReadyToBePreserved);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnReadyToBePreserved_WhenDue()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active.GetDisplayValue() && t.TagType == TagType.Standard);

                Assert.IsTrue(stdTagActiveDto.ReadyToBePreserved);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldReturnNoElements_WhenThereIsNoTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery("NO"), default);
                AssertCount(result.Data, 0);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnTagNo()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var tagNoStartsWith = $"{_testDataSet.StdTagPrefix}-0";
                var filter = new Filter {TagNoStartsWith = tagNoStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);

                var tags = result.Data.Tags.ToList();
                AssertCount(result.Data, 1);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.TagNo.StartsWith(tagNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnCommPkg()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var commPkgNoStartsWith = $"{_testDataSet.CommPkgPrefix}-0";
                var filter = new Filter {CommPkgNoStartsWith = commPkgNoStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.CommPkgNo.StartsWith(commPkgNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnMcPkg()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var mcPkgNoStartsWith = $"{_testDataSet.McPkgPrefix}-0";
                var filter = new Filter {McPkgNoStartsWith = mcPkgNoStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.McPkgNo.StartsWith(mcPkgNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnPurchaseOrder()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var purchaseOrderNoStartsWith = $"{_testDataSet.PoPrefix}-0";
                var filter = new Filter {PurchaseOrderNoStartsWith = purchaseOrderNoStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.PurchaseOrderNo.StartsWith(purchaseOrderNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnStorageArea()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var storageAreaStartsWith = $"{_testDataSet.StorageAreaPrefix}-0";
                var filter = new Filter {StorageAreaStartsWith = storageAreaStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.StorageArea.StartsWith(storageAreaStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnCallOff()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var callOffStartsWith = $"{_testDataSet.CallOffPrefix}-0";
                var filter = new Filter {CallOffStartsWith = callOffStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.CalloffNo.StartsWith(callOffStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnPreservationStatus()
        {
            var filter = new Filter {PreservationStatus = PreservationStatus.Active};
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);

                Assert.AreEqual(0, result.Data.Tags.Count());
            }
            
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);

                AssertCount(result.Data, 20);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(PreservationStatus.Active.GetDisplayValue(), tag.Status);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnPreservationStatusEqualInService()
        {
            var filter = new Filter { PreservationStatus = PreservationStatus.InService };
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);

                Assert.AreEqual(0, result.Data.Tags.Count());
            }

            SetInServiceOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);

                AssertCount(result.Data, 20);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(PreservationStatus.InService.GetDisplayValue(), tag.Status);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnOpenActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                tag.AddAction(new Action(Guid.Empty, TestPlant, "A", "Desc", null));
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                AssertCount(result.Data, 1);
                AssertActionStatus(result.Data, ActionStatus.HasOpen);
                
                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                AssertCount(result.Data, 0);
                
                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOverdue}), default);
                AssertCount(result.Data, 0);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnClosedActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                var action = new Action(Guid.Empty, TestPlant, "A", "Desc", null);
                action.Close(_timeProvider.UtcNow, context.Persons.First());
                tag.AddAction(action);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                AssertCount(result.Data, 0);
                
                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                AssertCount(result.Data, 1);
                AssertActionStatus(result.Data, ActionStatus.HasClosed);
                
                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOverdue}), default);
                AssertCount(result.Data, 0);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnOverdueActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                var action = new Action(Guid.Empty, TestPlant, "A", "Desc", _timeProvider.UtcNow.AddDays(-1));
                tag.AddAction(action);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                // when filtering on tag which has Open actions, tags with overdue actions is included
                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                AssertCount(result.Data, 1);
                AssertActionStatus(result.Data, ActionStatus.HasOverdue);
                var tagIdWithOpenAndOverdueAction = result.Data.Tags.Single().Id;
                
                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                AssertCount(result.Data, 0);
                
                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOverdue}), default);
                AssertCount(result.Data, 1);
                AssertActionStatus(result.Data, ActionStatus.HasOverdue);
                Assert.AreEqual(tagIdWithOpenAndOverdueAction, result.Data.Tags.Single().Id);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldNotGetAnyTags_WhenFilterOnDue_WhenPreservationNotStarted()
        {
            var filter = new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek}};
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 0);
            }

            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 0);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldGetTagsDueInThreeWeeks_WhenFilterOnDueWeekPlusThree()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks-3);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusTwo}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusThree}}), default);
                AssertCount(result.Data, 20);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek, DueFilterType.WeekPlusTwo, DueFilterType.WeekPlusThree
                        }
                    }), default);
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldGetTagsDueInTwoWeeks_WhenFilterOnDueWeekPlusTwo()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks-2);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusTwo}}), default);
                AssertCount(result.Data, 20);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusThree}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek, DueFilterType.WeekPlusTwo, DueFilterType.WeekPlusThree
                        }
                    }), default);
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldGetTagsDueNextWeek_WhenFilterOnDueNextWeek()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks-1);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusTwo}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusThree}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                AssertCount(result.Data, 20);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek, DueFilterType.WeekPlusTwo, DueFilterType.WeekPlusThree
                        }
                    }), default);
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldGetTagsDueThisWeek_WhenFilterOnDueThisWeek()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusTwo}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusThree}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                AssertCount(result.Data, 20);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek, DueFilterType.WeekPlusTwo, DueFilterType.WeekPlusThree
                        }
                    }), default);
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldGetTagsOverdue_WhenFilterOnOverdue()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks+1);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusTwo}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.WeekPlusThree}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue}}), default);
                AssertCount(result.Data, 20);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek, DueFilterType.WeekPlusTwo, DueFilterType.WeekPlusThree
                        }
                    }), default);
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnRequirementType()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter {RequirementTypeIds = new List<int>{_testDataSet.ReqType1.Id}};
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 10);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(_testDataSet.ReqType1.Code, tag.Requirements.Single().RequirementTypeCode);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnAreaCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var areaCode = $"{_testDataSet.AreaPrefix}-0";
                var filter = new Filter {AreaCodes = new List<string>{areaCode}};
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(areaCode, tag.AreaCode);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnDisciplineCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var diCode = $"{_testDataSet.DisciplinePrefix}-0";
                var filter = new Filter {DisciplineCodes = new List<string>{diCode}};
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(diCode, tag.DisciplineCode);
                }
            }
        }
        
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnResponsible()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter {ResponsibleIds = new List<int>{_testDataSet.Responsible1.Id}};
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 20);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(_testDataSet.Responsible1.Code, tag.ResponsibleCode);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnTagFunctionCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tfCode = $"{_testDataSet.TagFunctionPrefix}-0";
                var filter = new Filter {TagFunctionCodes = new List<string>{tfCode}};
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(tfCode, tag.TagFunctionCode);
                }
            }
        }
        
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnMode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter {ModeIds = new List<int>{_testDataSet.Mode1.Id}};
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 20);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(_testDataSet.Mode1.Title, tag.Mode);
                }
            }
        }
        
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnJourney()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter {JourneyIds = new List<int>{_testDataSet.Journey2With1Step.Id}};
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 10);
            }
        }
        
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnStep()
        {
            var filter = new Filter {StepIds = new List<int>{_testDataSet.Journey1With2Steps.Steps.First().Id}};
            IEnumerable<int> tagIdsToTransfer;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 10);
                tagIdsToTransfer = result.Data.Tags.Select(t => t.Id).Take(5);
            }

            StartPreservationOnAllTags();
            TransferTags(tagIdsToTransfer);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 5);
            }
        }
        
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterOnVoided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                tag.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {VoidedFilter = VoidedFilterType.NotVoided}), default);
                AssertCount(result.Data, 19);
                AssertIsVoided(result.Data, false);

                result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: new Filter {VoidedFilter = VoidedFilterType.Voided}), default);
                AssertCount(result.Data, 1);
                AssertIsVoided(result.Data, true);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldFilterWhenAllFiltersSet()
        {
            var filter = new Filter
            {
                PreservationStatus = PreservationStatus.NotStarted,
                RequirementTypeIds = new List<int> {_testDataSet.ReqType1.Id},
                AreaCodes = new List<string> {$"{_testDataSet.AreaPrefix}-0"},
                DisciplineCodes = new List<string> {$"{_testDataSet.DisciplinePrefix}-0"},
                ResponsibleIds = new List<int> {_testDataSet.Responsible1.Id},
                TagFunctionCodes = new List<string> {$"{_testDataSet.TagFunctionPrefix}-0"},
                ModeIds = new List<int> {_testDataSet.Mode1.Id},
                JourneyIds = new List<int> {_testDataSet.Journey1With2Steps.Id},
                StepIds = new List<int> {_testDataSet.Journey1With2Steps.Steps.First().Id},
                TagNoStartsWith = $"{_testDataSet.StdTagPrefix}-0",
                CommPkgNoStartsWith = $"{_testDataSet.CommPkgPrefix}-0",
                McPkgNoStartsWith = $"{_testDataSet.McPkgPrefix}-0",
                PurchaseOrderNoStartsWith = $"{_testDataSet.PoPrefix}-0",
                CallOffStartsWith = $"{_testDataSet.CallOffPrefix}-0",
                VoidedFilter = VoidedFilterType.NotVoided
            };
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);
                AssertCount(result.Data, 1);
            }
        }
                
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldSortOnTagNo()
        {
            // filter on specific journey. Will get 10 standard tags
            var filter = new Filter {JourneyIds = new List<int>{_testDataSet.Journey1With2Steps.Id}};

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var sorting = new Sorting(SortingDirection.Asc, SortingProperty.TagNo);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, sorting, filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-0", tags.First().TagNo);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-9", tags.Last().TagNo);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var sorting = new Sorting(SortingDirection.Desc, SortingProperty.TagNo);

                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, sorting, filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-9", tags.First().TagNo);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-0", tags.Last().TagNo);
            }
        }
                
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldSetIsNew_BeforeNewPeriodHasElapsed()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);
                Assert.IsTrue(result.Data.Tags.All(t => t.IsNew));
            }
        }
                
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldNotSetIsNew_AfterNewPeriodHasElapsed()
        {
            var timeSpan = new TimeSpan(_tagIsNewHours+1, 0, 0);
            _timeProvider.Elapse(timeSpan);
 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(_query, default);
                Assert.IsFalse(result.Data.Tags.Any(t => t.IsNew));
            }
        }
        
        [TestMethod]
        public async Task HandleGetTagsQuery_ShouldNotReturnNextModeOrResponsible_WhenTagCantBeTransferred()
        {
            int tagId;
            var tagTitle = "D43CDE9C5568";
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var project = context.Projects.Single(p => p.Name == _testDataSet.Project1.Name);
                var step = context.Steps.Single(s => s.Id == _testDataSet.Journey1With2Steps.Steps.ElementAt(0).Id);
                var reqDef = context.RequirementDefinitions.Single(rd =>
                    rd.Id == _testDataSet.ReqType1.RequirementDefinitions.ElementAt(0).Id);

                var tag = new Tag(TestPlant, TagType.SiteArea, Guid.NewGuid(), tagTitle, "Description", step,
                    new List<TagRequirement>
                    {
                        new TagRequirement(TestPlant, _testDataSet.IntervalWeeks, reqDef)
                    });
                tag.StartPreservation();
                project.AddTag(tag);

                context.SaveChangesAsync().Wait();
                tagId = tag.Id;
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter {TagNoStartsWith = tagTitle};

                var dut = new GetTagsQueryHandler(context, _apiOptionsMock.Object);
                var result = await dut.Handle(new GetTagsQuery(_testDataSet.Project1.Name, filter: filter), default);

                var tagDto = result.Data.Tags.Single(t => t.Id == tagId);

                Assert.IsNull(tagDto.NextMode);
                Assert.IsNull(tagDto.NextResponsibleCode);
            }
        }

        private void TransferTags(IEnumerable<int> tagIds)
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                foreach (var tagId in tagIds)
                {
                    var tag = context.Tags.Single(t => t.Id == tagId);
                    var journey = context.Journeys.Include(j => j.Steps).Single(j => j.Steps.Any(s => s.Id == tag.StepId));
                    tag.Transfer(journey);
                }
                context.SaveChangesAsync().Wait();
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

        private void SetInServiceOnAllTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tags = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).ToList();
                tags.ForEach(t =>
                {
                    t.StartPreservation();
                    t.SetInService();
                });
                context.SaveChangesAsync().Wait();
            }
        }

        private void TransferAllStandardTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journeys = context.Journeys.Include(j => j.Steps).ToList();
                var standardTags = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods)
                    .Where(t => t.TagType == TagType.Standard).ToList();
                foreach (var standardTag in standardTags)
                {
                    var journey = journeys.Single(j => j.Steps.Any(s => s.Id == standardTag.StepId));
                    standardTag.Transfer(journey);
                }
                context.SaveChangesAsync().Wait();
            }
        }

        private void CompleteAllTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journeys = context.Journeys.Include(j => j.Steps).ToList();
                var tags = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).ToList();
                foreach (var tag in tags)
                {
                    var journey = journeys.Single(j => j.Steps.Any(s => s.Id == tag.StepId));
                    if (tag.TagType == TagType.Standard)
                    {
                        tag.Transfer(journey);
                    }
                    tag.CompletePreservation(journey);
                }
                context.SaveChangesAsync().Wait();
            }
        }

        private void AssertCount(TagsResult data, int count)
        {
            Assert.AreEqual(count, data.MaxAvailable);
            Assert.AreEqual(count, data.Tags.Count());
        }

        private void AssertActionStatus(TagsResult data, ActionStatus actionStatus)
        {
            foreach (var tag in data.Tags)
            {
                Assert.AreEqual(actionStatus, tag.ActionStatus);
            }
        }

        private void AssertIsVoided(TagsResult data, bool isVoided)
        {
            foreach (var tag in data.Tags)
            {
                Assert.AreEqual(isVoided, tag.IsVoided);
            }
        }
    }
}
