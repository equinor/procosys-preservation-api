using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Query.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.ProjectAggregate
{
    [TestClass]
    public class GetAllTagsInProjectQueryHandlerTests
    {
        private const string ModeTitle = "Hookup";
        private const string ResponsibleCode = "EQC";
        private DateTime _utcNow;
        private const int IntervalWeeks = 4;
        private const string ProjectName = "ProjectX";
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<IResponsibleRepository> _respRepositoryMock;
        private Mock<ITimeService> _timeServiceMock;
        private GetAllTagsInProjectQueryHandler _dut;
        private GetAllTagsInProjectQuery _query;
        private List<Tag> _tags;
        private Tag _tagNotStartedPreservation;
        private Tag _tagStartedPreservation;

        [TestInitialize]
        public void Setup()
        {
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);

            var stepId = 2;
            var modeId = 12;
            var respId = 22;
            var plant = "PCS$TESTPLANT";
            
            var modeMock = new Mock<Mode>(plant, ModeTitle);
            modeMock.SetupGet(m => m.Id).Returns(modeId);

            var respMock = new Mock<Responsible>(plant, ResponsibleCode);
            respMock.SetupGet(r => r.Id).Returns(respId);
            
            var stepMock = new Mock<Step>(plant, modeMock.Object, respMock.Object);
            stepMock.SetupGet(s => s.Id).Returns(stepId);

            _tagNotStartedPreservation = new Tag(
                plant,
                "TagNo1",
                "Desc",
                "AreaCode",
                "Calloff",
                "DisciplineCode",
                "McPkgNo",
                "CommPkgNo",
                "PoNo",
                "TagFunctionCode",
                stepMock.Object,
                new List<Requirement>
                {
                    new Requirement(plant, IntervalWeeks, new Mock<RequirementDefinition>().Object)
                });
            _tagStartedPreservation = new Tag(
                plant,
                "TagNo2",
                "Desc",
                "AreaCode",
                "Calloff",
                "DisciplineCode",
                "McPkgNo",
                "CommPkgNo",
                "PoNo",
                "TagFunctionCode",
                stepMock.Object,
                new List<Requirement>
                {
                    new Requirement(plant, IntervalWeeks, new Mock<RequirementDefinition>().Object)
                });

            _tagStartedPreservation.StartPreservation(_utcNow);
            _tags = new List<Tag>
            {
                _tagNotStartedPreservation,
                _tagStartedPreservation
            };

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock
                .Setup(x => x.GetAllTagsInProjectAsync(ProjectName))
                .Returns(Task.FromResult(_tags));
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(r
                    => r.GetStepsByStepIdsAsync(new List<int> {stepId}))
                .Returns(Task.FromResult(new List<Step> {stepMock.Object}));

            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeRepositoryMock
                .Setup(r
                    => r.GetByIdsAsync(new List<int> {modeId}))
                .Returns(Task.FromResult(new List<Mode> {modeMock.Object}));
            
            _respRepositoryMock = new Mock<IResponsibleRepository>();
            _respRepositoryMock
                .Setup(r
                    => r.GetByIdsAsync(new List<int> {respId}))
                .Returns(Task.FromResult(new List<Responsible> {respMock.Object}));
            
            _timeServiceMock = new Mock<ITimeService>();
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_utcNow);
            
            _dut = new GetAllTagsInProjectQueryHandler(
                _projectRepositoryMock.Object,
                _journeyRepositoryMock.Object,
                _modeRepositoryMock.Object,
                _respRepositoryMock.Object,
                _timeServiceMock.Object);

            _query = new GetAllTagsInProjectQuery(ProjectName);
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnsOkResult()
        {
            var result = await _dut.Handle(_query, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnsCorrectNumberOfItems()
        {
            var result = await _dut.Handle(_query, default);

            Assert.AreEqual(2, result.Data.Count());
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnsCorrectDto()
        {
            var result = await _dut.Handle(_query, default);

            var tag = _tags[0];
            var tagDto = result.Data.First();
            Assert.AreEqual(tag.AreaCode, tagDto.AreaCode);
            Assert.AreEqual(tag.Calloff, tagDto.CalloffNo);
            Assert.AreEqual(tag.CommPkgNo, tagDto.CommPkgNo);
            Assert.AreEqual(tag.DisciplineCode, tagDto.DisciplineCode);
            Assert.AreEqual(tag.Id, tagDto.Id);
            Assert.AreEqual(tag.IsAreaTag, tagDto.IsAreaTag);
            Assert.AreEqual(tag.IsVoided, tagDto.IsVoided);
            Assert.AreEqual(tag.McPkgNo, tagDto.McPkgNo);
            Assert.AreEqual(ModeTitle, tagDto.Mode);
            Assert.AreEqual(ResponsibleCode, tagDto.ResponsibleCode);
            Assert.AreEqual(tag.Description, tagDto.Description);
            Assert.AreEqual(tag.PurchaseOrderNo, tagDto.PurchaseOrderNo);

            Assert.AreEqual(tag.Status, tagDto.Status);
            Assert.AreEqual(tag.TagFunctionCode, tagDto.TagFunctionCode);
            Assert.AreEqual(tag.TagNo, tagDto.TagNo);
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldNotReturnsDueInfo_WhenPreservationNotStarted()
        {
            var result = await _dut.Handle(_query, default);

            var tagDto = result.Data.First(t => t.Status == PreservationStatus.NotStarted);
            var requirementDto = tagDto.Requirements.First();

            Assert.IsNull(tagDto.FirstUpcomingRequirement);
            Assert.IsFalse(requirementDto.NextDueTimeUtc.HasValue);
            Assert.AreEqual(0, requirementDto.NextDueWeeks);
            Assert.IsNull(requirementDto.NextDueAsYearAndWeek);
            Assert.AreEqual(PreservationStatus.NotStarted, tagDto.Status);
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnsDueInfo_WhenPreservationStarted()
        {
            var result = await _dut.Handle(_query, default);

            var tagDto = result.Data.First(t => t.Status == PreservationStatus.Active);
            var requirementDto = tagDto.Requirements.First();

            Assert.IsTrue(requirementDto.NextDueTimeUtc.HasValue);
            Assert.AreEqual(IntervalWeeks, requirementDto.NextDueWeeks);
            Assert.IsNotNull(requirementDto.NextDueAsYearAndWeek);
            Assert.AreEqual(PreservationStatus.Active, tagDto.Status);
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnsCorrectFirstUpcomingRequirement_WhenPreservationStarted()
        {
            var result = await _dut.Handle(_query, default);

            var tagDto = result.Data.First(t => t.Status == PreservationStatus.Active);
            var requirementDto = tagDto.Requirements.First();

            Assert.IsNotNull(tagDto.FirstUpcomingRequirement);
            Assert.AreEqual(requirementDto, tagDto.FirstUpcomingRequirement);
        }

        [TestMethod]
        public async Task HandleGetAllTagsInProjectQuery_ShouldReturnsNoElements_WhenThereIsNoTags()
        {
            _projectRepositoryMock
                .Setup(x => x.GetAllTagsInProjectAsync(ProjectName))
                .Returns(Task.FromResult(new List<Tag>()));

            var result = await _dut.Handle(_query, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
