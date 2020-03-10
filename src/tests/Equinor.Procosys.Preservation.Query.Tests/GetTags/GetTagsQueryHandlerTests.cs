using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
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
        private string _journeyTitle = "J1";
        private string _modeTitleStep1 = "M1";
        private string _respCodeStep1 = "R1";
        private string _modeTitleStep2 = "M2";
        private string _respCodeStep2 = "R2";
        private string _reqTypeCode = "ROT";
        private int _intervalWeeks = 2;
        private GetTagsQuery _query;
        private string _stdTagPrefix = "StdTagNo";
        private string _siteTagPrefix = "SiteTagNo";
        private Sorting _sorting;
        private Filter _filter;
        private Paging _paging;

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

                _sorting = new Sorting(SortingDirection.Asc, SortingColumn.TagNo);
                _filter = new Filter(_projectName, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
                _paging = new Paging(0, 5);
                _query = new GetTagsQuery(_sorting, _filter, _paging);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectNumberOfMaxAvailable()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                // 30 tags added in setup, but 20 of them in project PX
                Assert.AreEqual(20, result.Data.MaxAvailable);
            }
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnCorrectDto()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var result = await dut.Handle(_query, default);

                var tagDto = result.Data.Tags.First();
                var tag = context.Tags.Single(t => t.TagNo == tagDto.TagNo);
                Assert.AreEqual(tag.AreaCode, tagDto.AreaCode);
                Assert.AreEqual(tag.Calloff, tagDto.CalloffNo);
                Assert.AreEqual(tag.CommPkgNo, tagDto.CommPkgNo);
                Assert.AreEqual(tag.DisciplineCode, tagDto.DisciplineCode);
                Assert.AreEqual(tag.Id, tagDto.Id);
                Assert.AreEqual(tag.IsVoided, tagDto.IsVoided);
                Assert.AreEqual(tag.McPkgNo, tagDto.McPkgNo);
                Assert.AreEqual(_modeTitleStep1, tagDto.Mode);
                Assert.AreEqual(_respCodeStep1, tagDto.ResponsibleCode);
                Assert.AreEqual(tag.Description, tagDto.Description);
                Assert.AreEqual(tag.PurchaseOrderNo, tagDto.PurchaseOrderNo);

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

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
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
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnNoElements_WhenThereIsNoTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new GetTagsQueryHandler(context);
                var filter = new Filter("NO", null, null, null, null, null, null, null, null, null, null, null, null, null, null);

                var result = await dut.Handle(new GetTagsQuery(_sorting, filter, _paging), default);

                Assert.AreEqual(0, result.Data.Tags.Count());
            }
        }

        private void StartPreservationOnAllTags()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var tags = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).ToList();
                tags.ForEach(t => t.StartPreservation());
                context.SaveChanges();
            }
        }
    }
}
