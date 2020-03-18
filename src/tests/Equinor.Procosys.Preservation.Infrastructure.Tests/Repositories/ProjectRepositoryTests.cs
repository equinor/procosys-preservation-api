using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class ProjectRepositoryTests : RepositoryTestBase
    {
        private const string ProjectNameWithTags = "ProjectName1";
        private const string ProjectNameWithoutTags = "ProjectName2";
        private List<Project> _projects;
        private const int TestTagId = 71;
        private Mock<Tag> _testTagMock;
        private Mock<DbSet<Project>> _dbSetMock;

        private ProjectRepository _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(x => x.Schema).Returns(TestPlant);

            var responsibleMock = new Mock<Responsible>();
            responsibleMock.SetupGet(x => x.Schema).Returns(TestPlant);

            var step = new Step(TestPlant, "S", modeMock.Object, responsibleMock.Object);
            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Schema).Returns(TestPlant);
            var requirements = new List<Requirement>
            {
                new Requirement(TestPlant, 1, rdMock.Object),
                new Requirement(TestPlant, 2, rdMock.Object),
                new Requirement(TestPlant, 4, rdMock.Object)
            };

            var project1 = new Project(TestPlant, ProjectNameWithTags, "Desc1");
            project1.AddTag(new Tag(TestPlant, TagType.Standard, "TagNo1", "Desc", "A", "CO", "Di", "MNo", "CNo", "PO", "R", "SA", "TF", step, requirements));
            project1.AddTag(new Tag(TestPlant, TagType.Standard, "TagX", "Desc", "A", "CO", "Di", "MNo", "CNo", "PO", "R", "SA", "TF", step, requirements));
            _testTagMock = new Mock<Tag>();
            _testTagMock.SetupGet(t => t.Id).Returns(TestTagId);
            _testTagMock.SetupGet(t => t.Schema).Returns(TestPlant);
            project1.AddTag(_testTagMock.Object);
            
            var project2 = new Project(TestPlant, ProjectNameWithoutTags, "Desc2");

            _projects = new List<Project> {project1, project2};
            
            _dbSetMock = _projects.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Projects)
                .Returns(_dbSetMock.Object);

            _dut = new ProjectRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetAllTagsInProject_Returns3Tags_WhenProjectHas3Tags()
        {
            var result = await _dut.GetAllTagsInProjectAsync(ProjectNameWithTags);

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task GetAllTagsInProject_ReturnsZeroTags_WhenProjectHasNoTags()
        {
            var result = await _dut.GetAllTagsInProjectAsync(ProjectNameWithoutTags);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetByName_ReturnsProjectWith3Tags()
        {
            var result = await _dut.GetByNameAsync(ProjectNameWithTags);

            Assert.AreEqual(ProjectNameWithTags, result.Name);
            Assert.AreEqual(3, result.Tags.Count);
        }

        [TestMethod]
        public async Task GetByName_ReturnsProjectWithNoTags()
        {
            var result = await _dut.GetByNameAsync(ProjectNameWithoutTags);

            Assert.AreEqual(ProjectNameWithoutTags, result.Name);
            Assert.AreEqual(0, result.Tags.Count);
        }

        [TestMethod]
        public async Task GetTagByTagId_ReturnsTag()
        {
            var result = await _dut.GetTagByTagIdAsync(TestTagId);

            Assert.AreEqual(TestTagId, result.Id);
        }

        [TestMethod]
        public async Task GetTagByTagIds_ReturnsTag()
        {
            var result = await _dut.GetTagsByTagIdsAsync(new List<int>{ TestTagId });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(TestTagId, result.First().Id);
        }
    }
}
