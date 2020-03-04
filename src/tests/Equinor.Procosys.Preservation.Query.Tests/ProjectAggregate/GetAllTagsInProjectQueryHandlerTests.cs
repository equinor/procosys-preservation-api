using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.ProjectAggregate;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.ProjectAggregate
{
    [TestClass]
    public class GetAllTagsInProjectQueryHandlerTests : ReadOnlyTestsBase
    {
        private string _projectName = "PX";
        private string _journeyTitle = "J1";
        private string _modeTitleStep1 = "M1";
        private string _respCodeStep1 = "R1";
        private string _modeTitleStep2 = "M2";
        private string _respCodeStep2 = "R2";
        private string _reqTypeCode = "ROT";
        private Mock<ITimeService> _timeServiceMock;
        private int _intervalWeeks = 2;
        private GetAllTagsInProjectQuery _query;
        private string _stdTagPrefix = "StdTagNo";
        private string _siteTagPrefix = "SiteTagNo";
        private DateTime _startedPreservationUtc;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider))
            {
                AddPerson(context, _currentUserOid, "Ole", "Lukkøye");

                var projectPx = AddProject(context, _projectName, "Project description");
                var projectAnother = AddProject(context, "Another", "Project description");
                
                var journeyWith2Steps = AddJourneyWithStep(context, _journeyTitle, 
                    AddMode(context, _modeTitleStep1), 
                    AddResponsible(context, _respCodeStep1));
                journeyWith2Steps.AddStep(new Step(TestPlant, AddMode(context, _modeTitleStep2), AddResponsible(context, _respCodeStep2)));
                context.SaveChanges();

                var reqType = AddRequirementTypeWith1DefWithoutField(context, _reqTypeCode, "D1");

                for (var i = 0; i < 10; i++)
                {
                    var tag = new Tag(TestPlant,
                        TagType.Standard,
                        $"{_stdTagPrefix}-{i}",
                        "Stand",
                        "AreaCode",
                        "Calloff",
                        "DisciplineCode",
                        "McPkgNo",
                        "CommPkgNo",
                        "PurchaseOrderNo",
                        "Remark",
                        "TagFunctionCode",
                        journeyWith2Steps.Steps.ElementAt(0),
                        new List<Requirement>
                        {
                            new Requirement(TestPlant, _intervalWeeks, reqType.RequirementDefinitions.ElementAt(0))
                        });
                
                    projectPx.AddTag(tag);
                }
                for (var i = 0; i < 10; i++)
                {
                    var tag = new Tag(TestPlant,
                        TagType.SiteArea,
                        $"{_siteTagPrefix}-{i}",
                        "Stand",
                        "AreaCode",
                        "Calloff",
                        "DisciplineCode",
                        "McPkgNo",
                        "CommPkgNo",
                        "PurchaseOrderNo",
                        "Remark",
                        "TagFunctionCode",
                        journeyWith2Steps.Steps.ElementAt(0),
                        new List<Requirement>
                        {
                            new Requirement(TestPlant, _intervalWeeks, reqType.RequirementDefinitions.ElementAt(0))
                        });
                
                    projectPx.AddTag(tag);
                }

                for (var i = 0; i < 10; i++)
                {
                    var tag = new Tag(TestPlant,
                        TagType.Standard,
                        $"Another-{i}",
                        "Stand",
                        "AreaCode",
                        "Calloff",
                        "DisciplineCode",
                        "McPkgNo",
                        "CommPkgNo",
                        "PurchaseOrderNo",
                        "Remark",
                        "TagFunctionCode",
                        journeyWith2Steps.Steps.ElementAt(0),
                        new List<Requirement>
                        {
                            new Requirement(TestPlant, _intervalWeeks, reqType.RequirementDefinitions.ElementAt(0))
                        });
                
                    projectAnother.AddTag(tag);
                }
                context.SaveChanges();

                _timeServiceMock = new Mock<ITimeService>();
                _startedPreservationUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
                _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_startedPreservationUtc);
                _query = new GetAllTagsInProjectQuery(_projectName);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetAllTagsInProjectQueryHandler(context, _timeServiceMock.Object);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectNumberOfItems()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetAllTagsInProjectQueryHandler(context, _timeServiceMock.Object);
                var result = await dut.Handle(_query, default);

                // 30 tags added in setup, but 20 of them in project PX
                Assert.AreEqual(20, result.Data.Count());
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectDto()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetAllTagsInProjectQueryHandler(context, _timeServiceMock.Object);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.First();
                var tag = context.Tags.Single(t => t.TagNo == tagDto.TagNo);
                Assert.AreEqual(tag.AreaCode, tagDto.AreaCode);
                Assert.AreEqual(tag.Calloff, tagDto.CalloffNo);
                Assert.AreEqual(tag.CommPkgNo, tagDto.CommPkgNo);
                Assert.AreEqual(tag.DisciplineCode, tagDto.DisciplineCode);
                Assert.AreEqual(tag.Id, tagDto.Id);
                Assert.AreEqual(tag.TagType, tagDto.TagType);
                Assert.AreEqual(tag.IsVoided, tagDto.IsVoided);
                Assert.AreEqual(tag.McPkgNo, tagDto.McPkgNo);
                Assert.AreEqual(_modeTitleStep1, tagDto.Mode);
                Assert.AreEqual(_respCodeStep1, tagDto.ResponsibleCode);
                Assert.AreEqual(tag.Description, tagDto.Description);
                Assert.AreEqual(tag.PurchaseOrderNo, tagDto.PurchaseOrderNo);
                Assert.AreEqual(tag.Remark, tagDto.Remark);

                Assert.AreEqual(tag.Status, tagDto.Status);
                Assert.AreEqual(tag.TagFunctionCode, tagDto.TagFunctionCode);
                Assert.AreEqual(tag.TagNo, tagDto.TagNo);

                Assert.AreEqual(_reqTypeCode, tagDto.Requirements.First().RequirementTypeCode);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldNotReturnDueInfo_WhenPreservationNotStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetAllTagsInProjectQueryHandler(context, _timeServiceMock.Object);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.First(t => t.Status == PreservationStatus.NotStarted);
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

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetAllTagsInProjectQueryHandler(context, _timeServiceMock.Object);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.First(t => t.Status == PreservationStatus.Active);
                var requirementDto = tagDto.Requirements.First();

                Assert.IsTrue(requirementDto.NextDueTimeUtc.HasValue);
                Assert.AreEqual(_intervalWeeks, requirementDto.NextDueWeeks);
                Assert.IsNotNull(requirementDto.NextDueAsYearAndWeek);
                Assert.AreEqual(PreservationStatus.Active, tagDto.Status);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldNotReturnReadyToBeTransferred_BeforePreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetAllTagsInProjectQueryHandler(context, _timeServiceMock.Object);
                var result = await dut.Handle(_query, default);
                var tagNotStartedDto = result.Data.First(t => t.Status == PreservationStatus.NotStarted);
                Assert.IsFalse(tagNotStartedDto.ReadyToBeTransferred);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnReadyToBeTransferredForStandardTags_WhenPreservationStarted()
        {
            StartPreservationOnAllTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetAllTagsInProjectQueryHandler(context, _timeServiceMock.Object);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.First(t => t.Status == PreservationStatus.Active && t.TagType == TagType.Standard);
                var siteTagActiveDto = result.Data.First(t => t.Status == PreservationStatus.Active && t.TagType == TagType.SiteArea);

                Assert.IsTrue(stdTagActiveDto.ReadyToBeTransferred);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeTransferred);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldNotReturnReadyToBeTransferredForStandardTags_WhenTranasferedToLastStep()
        {
            StartPreservationOnAllTags();
            
            TransferAllStandardTags();

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetAllTagsInProjectQueryHandler(context, _timeServiceMock.Object);
                var result = await dut.Handle(_query, default);

                var stdTagActiveDto = result.Data.First(t => t.Status == PreservationStatus.Active && t.TagType == TagType.Standard);
                var siteTagActiveDto = result.Data.First(t => t.Status == PreservationStatus.Active && t.TagType == TagType.SiteArea);

                Assert.IsFalse(stdTagActiveDto.ReadyToBeTransferred);
                Assert.IsFalse(siteTagActiveDto.ReadyToBeTransferred);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnNoElements_WhenThereIsNoTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetAllTagsInProjectQueryHandler(context, _timeServiceMock.Object);
                var result = await dut.Handle(new GetAllTagsInProjectQuery("NO"), default);

                Assert.AreEqual(0, result.Data.Count());
            }
        }

        private void StartPreservationOnAllTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var tags = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).ToList();
                tags.ForEach(t => t.StartPreservation(_startedPreservationUtc));
                context.SaveChanges();
            }
        }

        private void TransferAllStandardTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
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
    }
}
