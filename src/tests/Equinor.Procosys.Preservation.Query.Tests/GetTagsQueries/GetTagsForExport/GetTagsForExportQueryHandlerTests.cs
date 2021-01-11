using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagsQueries;
using Equinor.Procosys.Preservation.Query.GetTagsQueries.GetTagsForExport;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagsQueries.GetTagsForExport
{
    [TestClass]
    public class GetTagsForExportQueryHandlerTests : ReadOnlyTestsBase
    {
        private GetTagsForExportQuery _query;
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                _query = new GetTagsForExportQuery(_testDataSet.Project1.Name);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldReturnCorrectCounts()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.NotStarted.GetDisplayValue());

                Assert.IsFalse(tagDto.NextDueWeeks.HasValue);
                Assert.IsNull(tagDto.NextDueAsYearAndWeek);
                Assert.AreEqual(PreservationStatus.NotStarted.GetDisplayValue(), tagDto.Status);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldReturnDueInfo_WhenPreservationStarted()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active.GetDisplayValue());

                Assert.AreEqual(_testDataSet.IntervalWeeks, tagDto.NextDueWeeks);
                Assert.IsNotNull(tagDto.NextDueAsYearAndWeek);
                Assert.AreEqual(PreservationStatus.Active.GetDisplayValue(), tagDto.Status);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldReturnNoElements_WhenThereIsNoTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery("NO"), default);
                Assert.AreEqual(0, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnTagNo()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var tagNoStartsWith = $"{_testDataSet.StdTagPrefix}-0";
                var filter = new Filter {TagNoStartsWith = tagNoStartsWith};

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
        public async Task HandleGetTagsForExportQuery_ShouldFilterOnCommPkg()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var commPkgNoStartsWith = $"{_testDataSet.CommPkgPrefix}-0";
                var filter = new Filter {CommPkgNoStartsWith = commPkgNoStartsWith};

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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var mcPkgNoStartsWith = $"{_testDataSet.McPkgPrefix}-0";
                var filter = new Filter {McPkgNoStartsWith = mcPkgNoStartsWith};

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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var purchaseOrderNoStartsWith = $"{_testDataSet.PoPrefix}-0";
                var filter = new Filter {PurchaseOrderNoStartsWith = purchaseOrderNoStartsWith};

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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var storageAreaStartsWith = $"{_testDataSet.StorageAreaPrefix}-0";
                var filter = new Filter {StorageAreaStartsWith = storageAreaStartsWith};

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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var callOffStartsWith = $"{_testDataSet.CallOffPrefix}-0";
                var filter = new Filter {CallOffStartsWith = callOffStartsWith};

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
            var filter = new Filter {PreservationStatus = PreservationStatus.Active};
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);

                Assert.AreEqual(0, result.Data.Tags.Count());
            }
            
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);

                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 20);
                foreach (var tag in tags)
                {
                    Assert.AreEqual(PreservationStatus.Active.GetDisplayValue(), tag.Status);
                }
                Assert.AreEqual(filter.PreservationStatus.GetDisplayValue(), result.Data.UsedFilter.PreservationStatus);
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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 1);
                AssertActionStatus(result.Data.Tags, ActionStatus.HasOpen);
                Assert.AreEqual(ActionStatus.HasOpen.GetDisplayValue(), result.Data.UsedFilter.ActionStatus);

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(ActionStatus.HasClosed.GetDisplayValue(), result.Data.UsedFilter.ActionStatus);
                
                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOverdue}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(ActionStatus.HasOverdue.GetDisplayValue(), result.Data.UsedFilter.ActionStatus);
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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(ActionStatus.HasOpen.GetDisplayValue(), result.Data.UsedFilter.ActionStatus);

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 1);
                AssertActionStatus(tags, ActionStatus.HasClosed);
                Assert.AreEqual(ActionStatus.HasClosed.GetDisplayValue(), result.Data.UsedFilter.ActionStatus);

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOverdue}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(ActionStatus.HasOverdue.GetDisplayValue(), result.Data.UsedFilter.ActionStatus);
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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                // when filtering on tag which has Open actions, tags with overdue actions is included
                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 1);
                AssertActionStatus(tags, ActionStatus.HasOverdue);
                Assert.AreEqual(ActionStatus.HasOpen.GetDisplayValue(), result.Data.UsedFilter.ActionStatus);
                var tagWithOpenAndOverdueAction = tags.Single().TagNo;
                
                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 0);
                Assert.AreEqual(ActionStatus.HasClosed.GetDisplayValue(), result.Data.UsedFilter.ActionStatus);

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {ActionStatus = ActionStatus.HasOverdue}), default);
                tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 1);
                AssertActionStatus(tags, ActionStatus.HasOverdue);
                Assert.AreEqual(tagWithOpenAndOverdueAction, tags.Single().TagNo);
                Assert.AreEqual(ActionStatus.HasOverdue.GetDisplayValue(), result.Data.UsedFilter.ActionStatus);
            }
        }

        [TestMethod]
        public async Task HandleGetTagsForExportQuery_ShouldNotGetAnyTags_WhenFilterOnDue_WhenPreservationNotStarted()
        {
            var filter = new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue, DueFilterType.ThisWeek, DueFilterType.NextWeek}};
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
            }

            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

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
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks-1);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 20);
                Assert.AreEqual(DueFilterType.NextWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.ThisWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue}}), default);
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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.NextWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 20);
                Assert.AreEqual(DueFilterType.ThisWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue}}), default);
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
            _timeProvider.ElapseWeeks(_testDataSet.IntervalWeeks+1);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.NextWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                Assert.AreEqual(result.Data.Tags.Count(), 0);
                Assert.AreEqual(DueFilterType.ThisWeek.GetDisplayValue(), result.Data.UsedFilter.DueFilters.ElementAt(0));

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.Overdue}}), default);
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
                var filter = new Filter {RequirementTypeIds = new List<int>{_testDataSet.ReqType1.Id}};
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 10);
                var expectedRequirementTitle = _testDataSet.ReqType1.RequirementDefinitions.Single().Title;
                foreach (var tag in tags)
                {
                    Assert.AreEqual(expectedRequirementTitle, tag.RequirementTitles);
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
                var filter = new Filter {AreaCodes = new List<string>{areaCode}};
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

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
                var filter = new Filter {DisciplineCodes = new List<string>{diCode}};
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

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
                var filter = new Filter {ResponsibleIds = new List<int>{_testDataSet.Responsible1.Id}};
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

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
                var filter = new Filter {TagFunctionCodes = new List<string>{tfCode}};
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

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
                var filter = new Filter {ModeIds = new List<int>{_testDataSet.Mode1.Id}};
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

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
                var filter = new Filter {JourneyIds = new List<int>{_testDataSet.Journey2With1Step.Id}};
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

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
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {VoidedFilter = VoidedFilterType.NotVoided}), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(tags.Count, 19);
                Assert.AreEqual(VoidedFilterType.NotVoided.GetDisplayValue(), result.Data.UsedFilter.VoidedFilter);
                AssertIsVoided(tags, false);

                result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: new Filter {VoidedFilter = VoidedFilterType.Voided}), default);
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
                PreservationStatus = PreservationStatus.NotStarted,
                RequirementTypeIds = new List<int> {_testDataSet.ReqType1.Id},
                AreaCodes = new List<string> {$"{_testDataSet.AreaPrefix}-0"},
                DisciplineCodes = new List<string> {$"{_testDataSet.DisciplinePrefix}-0"},
                ResponsibleIds = new List<int> {_testDataSet.Responsible1.Id},
                TagFunctionCodes = new List<string> {$"{_testDataSet.TagFunctionPrefix}-0"},
                ModeIds = new List<int> {_testDataSet.Mode1.Id},
                JourneyIds = new List<int> {_testDataSet.Journey1With2Steps.Id},
                TagNoStartsWith = $"{_testDataSet.StdTagPrefix}-0",
                CommPkgNoStartsWith = $"{_testDataSet.CommPkgPrefix}-0",
                McPkgNoStartsWith = $"{_testDataSet.McPkgPrefix}-0",
                PurchaseOrderNoStartsWith = $"{_testDataSet.PoPrefix}-0",
                CallOffStartsWith = $"{_testDataSet.CallOffPrefix}-0",
                VoidedFilter = VoidedFilterType.NotVoided
            };
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, filter: filter), default);
                Assert.AreEqual(result.Data.Tags.Count(), 1);
                Assert.AreEqual(filter.VoidedFilter.GetDisplayValue(), result.Data.UsedFilter.VoidedFilter);
                Assert.AreEqual(filter.PreservationStatus.GetDisplayValue(), result.Data.UsedFilter.PreservationStatus);
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
            var filter = new Filter {JourneyIds = new List<int>{_testDataSet.Journey1With2Steps.Id}};

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var sorting = new Sorting(SortingDirection.Asc, SortingProperty.TagNo);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, sorting, filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-0", tags.First().TagNo);
                Assert.AreEqual($"{_testDataSet.StdTagPrefix}-9", tags.Last().TagNo);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsForExportQueryHandler(context, _plantProvider);
                var sorting = new Sorting(SortingDirection.Desc, SortingProperty.TagNo);

                var result = await dut.Handle(new GetTagsForExportQuery(_testDataSet.Project1.Name, sorting, filter), default);
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
            Assert.AreEqual(_testDataSet.ReqType1.RequirementDefinitions.First().Title, tagDto.RequirementTitles);
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
            Assert.AreEqual(_query.Filter.ActionStatus.GetDisplayValue(), usedFilterDto.ActionStatus);
            Assert.AreEqual(0, usedFilterDto.AreaCodes.Count());
            Assert.IsNull(usedFilterDto.CallOffStartsWith);
            Assert.IsNull(usedFilterDto.CommPkgNoStartsWith);
            Assert.AreEqual(0, usedFilterDto.DisciplineCodes.Count());
            Assert.AreEqual(0, usedFilterDto.DueFilters.Count());
            Assert.AreEqual(0, usedFilterDto.JourneyTitles.Count());
            Assert.IsNull(usedFilterDto.McPkgNoStartsWith);
            Assert.AreEqual(0, usedFilterDto.ModeTitles.Count());
            Assert.AreEqual(_query.Filter.PreservationStatus.GetDisplayValue(), usedFilterDto.PreservationStatus);
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
