using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class ProjectRepositoryTests
    {
        private const string TestPlant = "PCS$TESTPLANT";
        private const string ProjectNameWithTags = "ProjectName1";
        private const string ProjectNameWithoutTags = "ProjectName2";
        private List<Project> _projects;
        private Mock<DbSet<Project>> _dbSetMock;
        private ContextHelper _contextHelper;

        [TestInitialize]
        public void Setup()
        {
            var step = new Mock<Step>().Object;
            var requirements = new List<Requirement>
            {
                new Mock<Requirement>().Object,
                new Mock<Requirement>().Object,
                new Mock<Requirement>().Object,
            };

            var project1 = new Project(TestPlant, ProjectNameWithTags, "Desc1");
            project1.AddTag(new Tag(TestPlant, "TagNo1", "Desc", "A", "CO", "Di", "MNo", "CNo", "PO", "TF", step, requirements));
            project1.AddTag(new Tag(TestPlant, "TagNo2", "Desc", "A", "CO", "Di", "MNo", "CNo", "PO", "TF", step, requirements));
            project1.AddTag(new Tag(TestPlant, "TagNo3", "Desc", "A", "CO", "Di", "MNo", "CNo", "PO", "TF", step, requirements));
            var project2 = new Project(TestPlant, ProjectNameWithoutTags, "Desc2");
            
            _projects = new List<Project> {project1, project2};
            
            _dbSetMock = _projects.AsQueryable().BuildMockDbSet();

            _contextHelper = new ContextHelper();
            _contextHelper
                .ContextMock
                .Setup(x => x.Projects)
                .Returns(_dbSetMock.Object);
        }

        [TestMethod]
        public async Task GetAllTagsInProject_Returns3Tags_WhenProjectHas3Tags()
        {
            var dut = new ProjectRepository(_contextHelper.ContextMock.Object);

            var result = await dut.GetAllTagsInProjectAsync(ProjectNameWithTags);

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task GetAllTagsInProject_ReturnsZeroTags_WhenProjectHasNoTags()
        {
            var dut = new ProjectRepository(_contextHelper.ContextMock.Object);

            var result = await dut.GetAllTagsInProjectAsync(ProjectNameWithoutTags);

            Assert.AreEqual(0, result.Count);
        }

        // TODO: Write tests for all methods
    }
}
