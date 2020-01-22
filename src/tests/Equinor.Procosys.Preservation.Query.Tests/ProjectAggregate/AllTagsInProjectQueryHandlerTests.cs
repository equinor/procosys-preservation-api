using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class AllTagsInProjectQueryHandlerTests
    {
        private const string ProjectName = "ProjectX";
        private Mock<IProjectRepository> _projectRepositoryMock;
        private AllTagsInProjectQueryHandler _dut;
        private AllTagsInProjectQuery _query;
        private List<Tag> _tags;

        [TestInitialize]
        public void Setup()
        {
            var plant = "PCS$TESTPLANT";
            var mode = new Mode(plant, "TestMode");
            var responsible = new Responsible(plant, "Responsible");
            var requirementDefinition = new RequirementDefinition(plant, "ReqDef", 2, 1);
            var requirements = new List<Requirement> { new Requirement(plant, 2, requirementDefinition) };
            var step = new Step(plant, mode, responsible);
            _tags = new List<Tag>
            {
                new Tag(plant, "TagNo1", "Desc", "AreaCode", "Calloff", "DisciplineCode", "McPkgNo", "CommPkgNo", "PoNo", "TagFunctionCode", step, requirements),
                new Tag(plant, "TagNo2", "Desc", "AreaCode", "Calloff", "DisciplineCode", "McPkgNo", "CommPkgNo", "PoNo", "TagFunctionCode", step, requirements),
                new Tag(plant, "TagNo3", "Desc", "AreaCode", "Calloff", "DisciplineCode", "McPkgNo", "CommPkgNo", "PoNo", "TagFunctionCode", step, requirements),
            };

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock
                .Setup(x => x.GetAllTagsInProjectAsync(ProjectName))
                .Returns(Task.FromResult(_tags));
            _dut = new AllTagsInProjectQueryHandler(_projectRepositoryMock.Object);
            _query = new AllTagsInProjectQuery(ProjectName); // todo
        }

        [TestMethod]
        public async Task Handle_ReturnsOkResult()
        {
            var result = await _dut.Handle(_query, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectNumberOfItems()
        {
            var result = await _dut.Handle(_query, default);

            Assert.AreEqual(3, result.Data.Count());
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectDto()
        {
            var result = await _dut.Handle(_query, default);

            var tag = _tags[0];
            var dto = result.Data.ElementAt(0);
            Assert.AreEqual(tag.AreaCode, dto.AreaCode);
            Assert.AreEqual(tag.Calloff, dto.CalloffNo);
            Assert.AreEqual(tag.CommPkgNo, dto.CommPkgNo);
            Assert.AreEqual(tag.DisciplineCode, dto.DisciplineCode);
            Assert.AreEqual(tag.Id, dto.Id);
            Assert.AreEqual(tag.IsAreaTag, dto.IsAreaTag);
            Assert.AreEqual(tag.IsVoided, dto.IsVoided);
            Assert.AreEqual(tag.McPkgNo, dto.McPkgNo);
            Assert.AreEqual(tag.Description, dto.Description);
            Assert.AreEqual(tag.PurchaseOrderNo, dto.PurchaseOrderNo);

            Assert.AreEqual(tag.Requirements.Count, dto.Requirements.Count());
            Assert.AreEqual(tag.Requirements.ElementAt(0).NextDueTimeUtc, dto.Requirements.ElementAt(0).NextDueTimeUtc);

            Assert.AreEqual(tag.Status, dto.Status);
            Assert.AreEqual(tag.StepId, dto.StepId);
            Assert.AreEqual(tag.TagFunctionCode, dto.TagFunctionCode);
            Assert.AreEqual(tag.TagNo, dto.TagNo);
        }

        [TestMethod]
        public async Task Handle_ReturnsNoElements_WhenThereIsNoTags()
        {
            _projectRepositoryMock
                .Setup(x => x.GetAllTagsInProjectAsync(ProjectName))
                .Returns(Task.FromResult(new List<Tag>()));

            var result = await _dut.Handle(_query, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
