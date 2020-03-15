using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTags;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTags
{
    [TestClass]
    public class GetTagsQueryHandlerTests : ReadOnlyTestsBase
    {
        private string _projectName = "PX";
        private string _journeyTitle1 = "J1";
        private string _journeyTitle2 = "J2";
        private string _mode1 = "M1";
        private string _resp1 = "R1";
        private string _mode2 = "M2";
        private string _resp2 = "R2";
        private string _reqType1Code = "ROT";
        private string _reqType2Code = "AREA";
        private int _intervalWeeks = 2;
        private GetTagsQuery _query;
        private string _stdTagPrefix = "StdTagNo";
        private string _siteTagPrefix = "SiteTagNo";
        private string _callOffPrefix = "CO";
        private string _areaPrefix = "AREA";
        private string _disciplinePrefix = "DI";
        private string _mcPkgPrefix = "MC";
        private string _commPkgPrefix = "COMM";
        private string _poPrefix = "PO";
        private string _tagFunctionPrefix = "TF";
        private int _journeyId1;
        private int _step1OnJourney1Id;
        private int _reqType1Id;
        private int _resp1Id;
        private int _mode1Id;
        private Person _person;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _person = AddPerson(context, _currentUserOid, "Ole", "Lukkøye");

                var projectPx = AddProject(context, _projectName, "Project description");
                var projectAnother = AddProject(context, "Another", "Project description");

                var mode1 = AddMode(context, _mode1);
                _mode1Id = mode1.Id;
                var responsible1 = AddResponsible(context, _resp1);
                _resp1Id = responsible1.Id;
                var mode2 = AddMode(context, _mode2);
                var responsible2 = AddResponsible(context, _resp2);

                var journey1With2Steps = AddJourneyWithStep(context, _journeyTitle1, mode1, responsible1);
                _journeyId1 = journey1With2Steps.Id;
                var journey2With1Steps = AddJourneyWithStep(context, _journeyTitle2, mode1, responsible1);
                var step1OnJourney2 = journey2With1Steps.Steps.ElementAt(0);
                var step1OnJourney1 = journey1With2Steps.Steps.ElementAt(0);
                _step1OnJourney1Id = step1OnJourney1.Id;

                var step2OnJourney1 = new Step(TestPlant, mode2, responsible2);

                journey1With2Steps.AddStep(step2OnJourney1);
                context.SaveChanges();

                var reqType1 = AddRequirementTypeWith1DefWithoutField(context, _reqType1Code, "D1");
                _reqType1Id = reqType1.Id;
                for (var i = 0; i < 10; i++)
                {
                    var tag = new Tag(TestPlant,
                        TagType.Standard,
                        $"{_stdTagPrefix}-{i}",
                        "Description",
                        $"{_areaPrefix}-{i}",
                        $"{_callOffPrefix}-{i}",
                        $"{_disciplinePrefix}-{i}",
                        $"{_mcPkgPrefix}-{i}",
                        $"{_commPkgPrefix}-{i}",
                        $"{_poPrefix}-{i}",
                        "Remark",
                        $"{_tagFunctionPrefix}-{i}",
                        step1OnJourney1,
                        new List<Requirement>
                        {
                            new Requirement(TestPlant, _intervalWeeks, reqType1.RequirementDefinitions.ElementAt(0))
                        });
                
                    projectPx.AddTag(tag);
                }

                var reqType2 = AddRequirementTypeWith1DefWithoutField(context, _reqType2Code, "D2");
                for (var i = 0; i < 10; i++)
                {
                    var tag = new Tag(TestPlant,
                        TagType.SiteArea,
                        $"{_siteTagPrefix}-{i}",
                        "Description",
                        $"{_areaPrefix}-{i}",
                        $"{_callOffPrefix}-{i}",
                        $"{_disciplinePrefix}-{i}",
                        $"{_mcPkgPrefix}-{i}",
                        $"{_commPkgPrefix}-{i}",
                        $"{_poPrefix}-{i}",
                        "Remark",
                        $"{_tagFunctionPrefix}-{i}",
                        step1OnJourney2,
                        new List<Requirement>
                        {
                            new Requirement(TestPlant, _intervalWeeks, reqType2.RequirementDefinitions.ElementAt(0))
                        });
                
                    projectPx.AddTag(tag);
                }

                for (var i = 0; i < 10; i++)
                {
                    var tag = new Tag(TestPlant,
                        TagType.Standard,
                        $"Another-{i}",
                        "Description",
                        "AreaCode",
                        "Calloff",
                        "DisciplineCode",
                        "McPkgNo",
                        "CommPkgNo",
                        "PurchaseOrderNo",
                        "Remark",
                        "TagFunctionCode",
                        journey1With2Steps.Steps.ElementAt(0),
                        new List<Requirement>
                        {
                            new Requirement(TestPlant, _intervalWeeks, reqType1.RequirementDefinitions.ElementAt(0))
                        });
                
                    projectAnother.AddTag(tag);
                }
                context.SaveChanges();

                _query = new GetTagsQuery(_projectName);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectCounts()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);
                // 30 tags added in setup, but 20 of them in project PX
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnPageSize()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var paging = new Paging(0, 5);
                var result = await dut.Handle(new GetTagsQuery(_projectName, paging: paging), default);
                Assert.AreEqual(20, result.Data.MaxAvailable);
                Assert.AreEqual(5, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnEmptyPageButMaxAvailable_WhenGettingBehindLastPage()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var paging = new Paging(1, 50);
                var result = await dut.Handle(new GetTagsQuery(_projectName, paging: paging), default);
                Assert.AreEqual(20, result.Data.MaxAvailable);
                Assert.AreEqual(0, result.Data.Tags.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectDto()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.TagNo.StartsWith(_stdTagPrefix));
                var tag = context.Tags.Single(t => t.Id == tagDto.Id);
                Assert.AreEqual(ActionStatus.None, tagDto.ActionStatus);
                Assert.AreEqual(tag.AreaCode, tagDto.AreaCode);
                Assert.AreEqual(tag.Calloff, tagDto.CalloffNo);
                Assert.AreEqual(tag.CommPkgNo, tagDto.CommPkgNo);
                Assert.AreEqual(tag.DisciplineCode, tagDto.DisciplineCode);
                Assert.AreEqual(tag.Id, tagDto.Id);
                Assert.AreEqual(tag.IsVoided, tagDto.IsVoided);
                Assert.AreEqual(tag.McPkgNo, tagDto.McPkgNo);
                Assert.AreEqual(_mode1, tagDto.Mode);
                Assert.AreEqual(_resp1, tagDto.ResponsibleCode);
                Assert.AreEqual(_mode2, tagDto.NextMode);
                Assert.AreEqual(_resp2, tagDto.NextResponsibleCode);
                Assert.AreEqual(tag.Description, tagDto.Description);
                Assert.AreEqual(tag.PurchaseOrderNo, tagDto.PurchaseOrderNo);
                Assert.AreEqual(tag.Status, tagDto.Status);
                Assert.AreEqual(tag.TagFunctionCode, tagDto.TagFunctionCode);
                Assert.AreEqual(tag.TagNo, tagDto.TagNo);
                Assert.AreEqual(_reqType1Code, tagDto.Requirements.First().RequirementTypeCode);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldNotReturnDueInfo_WhenPreservationNotStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.NotStarted);
                var requirementDto = tagDto.Requirements.First();

                Assert.IsFalse(requirementDto.NextDueTimeUtc.HasValue);
                Assert.IsFalse(requirementDto.NextDueWeeks.HasValue);
                Assert.IsNull(requirementDto.NextDueAsYearAndWeek);
                Assert.AreEqual(PreservationStatus.NotStarted, tagDto.Status);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnDueInfo_WhenPreservationStarted()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active);
                var requirementDto = tagDto.Requirements.First();

                Assert.IsTrue(requirementDto.NextDueTimeUtc.HasValue);
                Assert.AreEqual(_intervalWeeks, requirementDto.NextDueWeeks);
                Assert.IsNotNull(requirementDto.NextDueAsYearAndWeek);
                Assert.AreEqual(PreservationStatus.Active, tagDto.Status);
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectStatuses_BeforePreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);
                var tagNotStartedDto = result.Data.Tags.First(t => t.Status == PreservationStatus.NotStarted);
                Assert.IsFalse(tagNotStartedDto.ReadyToBePreserved);
                Assert.IsTrue(tagNotStartedDto.ReadyToBeStarted);
                Assert.IsFalse(tagNotStartedDto.ReadyToBeTransferred);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectStatuses_WhenPreservationStarted()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active && t.TagType == TagType.Standard);
                var siteTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active && t.TagType == TagType.SiteArea);

                Assert.IsTrue(stdTagActiveDto.ReadyToBeTransferred);
                Assert.IsFalse(stdTagActiveDto.ReadyToBeStarted);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeTransferred);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeStarted);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldNotReturnReadyToBeTransferredForStandardTags_WhenTransferredToLastStep()
        {
            StartPreservationOnAllTags();
            
            TransferAllStandardTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active && t.TagType == TagType.Standard);

                Assert.IsFalse(stdTagActiveDto.ReadyToBeTransferred);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldNotReturnReadyToBePreserved_BeforeDue()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active && t.TagType == TagType.Standard);

                Assert.IsFalse(stdTagActiveDto.ReadyToBePreserved);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnReadyToBePreserved_WhenDue()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_intervalWeeks);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.Tags.First(t => t.Status == PreservationStatus.Active && t.TagType == TagType.Standard);

                Assert.IsTrue(stdTagActiveDto.ReadyToBePreserved);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnNoElements_WhenThereIsNoTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery("NO"), default);
                AssertCount(result.Data, 0);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnTagNo()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var tagNoStartsWith = $"{_stdTagPrefix}-0";
                var filter = new Filter {TagNoStartsWith = tagNoStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);

                var tags = result.Data.Tags.ToList();
                AssertCount(result.Data, 1);
                foreach (var tag in tags)
                {
                    Assert.IsTrue(tag.TagNo.StartsWith(tagNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnCommPkg()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var commPkgNoStartsWith = $"{_commPkgPrefix}-0";
                var filter = new Filter {CommPkgNoStartsWith = commPkgNoStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.CommPkgNo.StartsWith(commPkgNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnMcPkg()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var mcPkgNoStartsWith = $"{_mcPkgPrefix}-0";
                var filter = new Filter {McPkgNoStartsWith = mcPkgNoStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.McPkgNo.StartsWith(mcPkgNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnPurchaseOrder()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var purchaseOrderNoStartsWith = $"{_poPrefix}-0";
                var filter = new Filter {PurchaseOrderNoStartsWith = purchaseOrderNoStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.PurchaseOrderNo.StartsWith(purchaseOrderNoStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnCallOff()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var callOffStartsWith = $"{_callOffPrefix}-0";
                var filter = new Filter {CallOffStartsWith = callOffStartsWith};

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.IsTrue(tag.CalloffNo.StartsWith(callOffStartsWith));
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnPreservationStatus()
        {
            var filter = new Filter {PreservationStatus = PreservationStatus.Active};
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);

                Assert.AreEqual(0, result.Data.Tags.Count());
            }
            
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);

                AssertCount(result.Data, 20);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(PreservationStatus.Active, tag.Status);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnNoneActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.None}), default);
                AssertCount(result.Data, 20);
                AssertActionStatus(result.Data, ActionStatus.None);
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                AssertCount(result.Data, 0);
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                AssertCount(result.Data, 0);
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasOverDue}), default);
                AssertCount(result.Data, 0);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnOpenActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                tag.AddAction(new Action(TestPlant, "A", "Desc", null));
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.None}), default);
                AssertCount(result.Data, 19);
                AssertActionStatus(result.Data, ActionStatus.None);
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                AssertCount(result.Data, 1);
                AssertActionStatus(result.Data, ActionStatus.HasOpen);
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                AssertCount(result.Data, 0);
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasOverDue}), default);
                AssertCount(result.Data, 0);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnClosedActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.First();
                var action = new Action(TestPlant, "A", "Desc", null);
                action.Close(_timeProvider.UtcNow, _person);
                tag.AddAction(action);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.None}), default);
                AssertCount(result.Data, 19);
                AssertActionStatus(result.Data, ActionStatus.None);

                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                AssertCount(result.Data, 0);
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                AssertCount(result.Data, 1);
                AssertActionStatus(result.Data, ActionStatus.HasClosed);
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasOverDue}), default);
                AssertCount(result.Data, 0);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnOverDueActions()
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
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.None}), default);
                AssertCount(result.Data, 19);
                AssertActionStatus(result.Data, ActionStatus.None);

                // when filtering on tag which has Open actions, tags with overdue actions is included
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasOpen}), default);
                AssertCount(result.Data, 1);
                AssertActionStatus(result.Data, ActionStatus.HasOverDue);
                var tagIdWithOpenAndOverDueAction = result.Data.Tags.Single().Id;
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasClosed}), default);
                AssertCount(result.Data, 0);
                
                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {ActionStatus = ActionStatus.HasOverDue}), default);
                AssertCount(result.Data, 1);
                AssertActionStatus(result.Data, ActionStatus.HasOverDue);
                Assert.AreEqual(tagIdWithOpenAndOverDueAction, result.Data.Tags.Single().Id);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldNotGetAnyTags_WhenFilterOnDue_WhenPreservationNotStarted()
        {
            var filter = new Filter {DueFilters = new List<DueFilterType>{DueFilterType.OverDue, DueFilterType.ThisWeek, DueFilterType.NextWeek}};
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 0);
            }

            _timeProvider.ElapseWeeks(_intervalWeeks);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 0);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldGetTagsDueNextWeek_WhenFilterOnDueNextWeek()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_intervalWeeks-1);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                AssertCount(result.Data, 20);

                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.OverDue}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_projectName,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.OverDue, DueFilterType.ThisWeek, DueFilterType.NextWeek
                        }
                    }), default);
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldGetTagsDueThisWeek_WhenFilterOnDueThisWeek()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_intervalWeeks);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                AssertCount(result.Data, 20);

                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.OverDue}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_projectName,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.OverDue, DueFilterType.ThisWeek, DueFilterType.NextWeek
                        }
                    }), default);
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldGetTagsOverDue_WhenFilterOnOverDue()
        {
            StartPreservationOnAllTags();
            _timeProvider.ElapseWeeks(_intervalWeeks+1);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.NextWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.ThisWeek}}), default);
                AssertCount(result.Data, 0);

                result = await dut.Handle(new GetTagsQuery(_projectName, filter: new Filter {DueFilters = new List<DueFilterType>{DueFilterType.OverDue}}), default);
                AssertCount(result.Data, 20);

                result = await dut.Handle(new GetTagsQuery(_projectName,
                    filter: new Filter
                    {
                        DueFilters = new List<DueFilterType>
                        {
                            DueFilterType.OverDue, DueFilterType.ThisWeek, DueFilterType.NextWeek
                        }
                    }), default);
                AssertCount(result.Data, 20);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnRequirementType()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter {RequirementTypeIds = new List<int>{_reqType1Id}};
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 10);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(_reqType1Code, tag.Requirements.Single().RequirementTypeCode);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnAreaCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var diCode = $"{_areaPrefix}-0";
                var filter = new Filter {AreaCodes = new List<string>{diCode}};
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(diCode, tag.AreaCode);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnDisciplineCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var diCode = $"{_disciplinePrefix}-0";
                var filter = new Filter {DisciplineCodes = new List<string>{diCode}};
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(diCode, tag.DisciplineCode);
                }
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnResponsible()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter {ResponsibleIds = new List<int>{_resp1Id}};
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 20);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(_resp1, tag.ResponsibleCode);
                }
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnTagFunctionCode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tfCode = $"{_tagFunctionPrefix}-0";
                var filter = new Filter {TagFunctionCodes = new List<string>{tfCode}};
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 2);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(tfCode, tag.TagFunctionCode);
                }
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnMode()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter {ModeIds = new List<int>{_mode1Id}};
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 20);
                foreach (var tag in result.Data.Tags)
                {
                    Assert.AreEqual(_mode1, tag.Mode);
                }
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnJourney()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var filter = new Filter {JourneyIds = new List<int>{_journeyId1}};
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 10);
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterOnStep()
        {
            var filter = new Filter {StepIds = new List<int>{_step1OnJourney1Id}};
            IEnumerable<int> tagIdsToTransfer;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 10);
                tagIdsToTransfer = result.Data.Tags.Select(t => t.Id).Take(5);
            }

            StartPreservationOnAllTags();
            TransferTags(tagIdsToTransfer);

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 5);
            }
        }
        
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldFilterWhenAllFiltersSet()
        {
            var filter = new Filter
            {
                PreservationStatus = PreservationStatus.NotStarted,
                RequirementTypeIds = new List<int> {_reqType1Id},
                AreaCodes = new List<string> {$"{_areaPrefix}-0"},
                DisciplineCodes = new List<string> {$"{_disciplinePrefix}-0"},
                ResponsibleIds = new List<int> {_resp1Id},
                TagFunctionCodes = new List<string> {$"{_tagFunctionPrefix}-0"},
                ModeIds = new List<int> {_mode1Id},
                JourneyIds = new List<int> {_journeyId1},
                StepIds = new List<int> {_step1OnJourney1Id},
                TagNoStartsWith = $"{_stdTagPrefix}-0",
                CommPkgNoStartsWith = $"{_commPkgPrefix}-0",
                McPkgNoStartsWith = $"{_mcPkgPrefix}-0",
                PurchaseOrderNoStartsWith = $"{_poPrefix}-0",
                CallOffStartsWith = $"{_callOffPrefix}-0"
            };
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);

                var result = await dut.Handle(new GetTagsQuery(_projectName, filter: filter), default);
                AssertCount(result.Data, 1);
            }
        }
                
        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldSortOnTagNo()
        {
            // filter on specific journey. Will get 10 standard tags
            var filter = new Filter {JourneyIds = new List<int>{_journeyId1}};

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var sorting = new Sorting(SortingDirection.Asc, SortingProperty.TagNo);

                var result = await dut.Handle(new GetTagsQuery(_projectName, sorting, filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
                Assert.AreEqual($"{_stdTagPrefix}-0", tags.First().TagNo);
                Assert.AreEqual($"{_stdTagPrefix}-9", tags.Last().TagNo);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var sorting = new Sorting(SortingDirection.Desc, SortingProperty.TagNo);

                var result = await dut.Handle(new GetTagsQuery(_projectName, sorting, filter), default);
                var tags = result.Data.Tags.ToList();
                Assert.AreEqual(10, tags.Count);
                Assert.AreEqual($"{_stdTagPrefix}-9", tags.First().TagNo);
                Assert.AreEqual($"{_stdTagPrefix}-0", tags.Last().TagNo);
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
                context.SaveChanges();
            }
        }

        private void StartPreservationOnAllTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tags = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).ToList();
                tags.ForEach(t => t.StartPreservation());
                context.SaveChanges();
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
                context.SaveChanges();
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
    }
}
