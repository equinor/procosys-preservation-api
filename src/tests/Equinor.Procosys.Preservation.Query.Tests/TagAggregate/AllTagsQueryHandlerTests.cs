using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.Query.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.TagAggregate
{
    [TestClass]
    public class AllTagsQueryHandlerTests
    {
        private Mock<ITagRepository> _tagRepositoryMock;
        private AllTagsQueryHandler _dut;
        private AllTagsQuery _query;
        private List<Tag> _tags;

        [TestInitialize]
        public void Setup()
        {
            _tagRepositoryMock = new Mock<ITagRepository>();
            _dut = new AllTagsQueryHandler(_tagRepositoryMock.Object);
            _query = new AllTagsQuery();
            var plant = "PCS$TESTPLANT";
            var mode = new Mode(plant, "TestMode");
            var responsible = new Responsible(plant, "Responsible");
            var step = new Step(plant, mode, responsible);
            var requirementDefinition = new RequirementDefinition(plant, "ReqDef", 2, 1);
            var requirements = new List<Requirement> { new Requirement(plant, 2, requirementDefinition) };
            _tags = new List<Tag>
            {
                new Tag("PCS$TESTPLANT", "TagNo1", "PoNo", "AreaCode", "CalloffNo", "DisciplineCode", "McPkgNo", "CommPkgNo", "PoNo", "TagFunctionCode", step, requirements),
                new Tag("PCS$TESTPLANT", "TagNo2", "PoNo", "AreaCode", "CalloffNo", "DisciplineCode", "McPkgNo", "CommPkgNo", "PoNo", "TagFunctionCode", step, requirements),
                new Tag("PCS$TESTPLANT", "TagNo3", "PoNo", "AreaCode", "CalloffNo", "DisciplineCode", "McPkgNo", "CommPkgNo", "PoNo", "TagFunctionCode", step, requirements),
            };
        }

        [TestMethod]
        public async Task Handle_ReturnsOkResult()
        {
            _tagRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(_tags));

            var result = await _dut.Handle(_query, default);

            Assert.AreEqual(ResultType.Ok, result.ResultType);
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectNumberOfItems()
        {
            _tagRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(_tags));

            var result = await _dut.Handle(_query, default);

            Assert.AreEqual(3, result.Data.Count());
        }

        [TestMethod]
        public async Task Handle_ReturnsCorrectDto()
        {
            _tagRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(_tags));

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
            Assert.AreEqual(tag.ProjectName, dto.ProjectName);
            Assert.AreEqual(tag.PurchaseOrderNo, dto.PurchaseOrderNo);

            Assert.AreEqual(tag.Requirements.Count, dto.Requirements.Count());
            Assert.AreEqual(tag.Requirements.ElementAt(0).Id, dto.Requirements.ElementAt(0).Id);
            Assert.AreEqual(tag.Requirements.ElementAt(0).IntervalWeeks, dto.Requirements.ElementAt(0).IntervalWeeks);
            Assert.AreEqual(tag.Requirements.ElementAt(0).IsVoided, dto.Requirements.ElementAt(0).IsVoided);
            Assert.AreEqual(tag.Requirements.ElementAt(0).RequirementDefinitionId, dto.Requirements.ElementAt(0).RequirementDefinitionId);

            Assert.AreEqual(tag.Status, dto.Status);
            Assert.AreEqual(tag.StepId, dto.StepId);
            Assert.AreEqual(tag.TagFunctionCode, dto.TagFunctionCode);
            Assert.AreEqual(tag.TagNo, dto.TagNo);
        }

        [TestMethod]
        public async Task Handle_ReturnsNoElements_WhenThereIsNoTags()
        {
            _tagRepositoryMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(new List<Tag>()));

            var result = await _dut.Handle(_query, default);

            Assert.AreEqual(0, result.Data.Count());
        }
    }
}
