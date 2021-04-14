using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.Repositories;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class ProjectRepositoryTests : RepositoryTestBase
    {
        private const string ProjectNameWithTags = "ProjectName1";
        private const string ProjectNameWithoutTags = "ProjectName2";
        private const int StepId = 61;
        private const int StandardTagId = 71;
        private const int PoTagId = 72;
        private const string StandardTagNo = "TagNo1";
        private const string PoTagNo = "TagNo2";

        private ProjectRepository _dut;
        private Tag _standardTagWith3Reqs;
        private Mock<DbSet<Tag>> _tagsSetMock;
        private Mock<DbSet<TagRequirement>> _reqsSetMock;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(x => x.Plant).Returns(TestPlant);

            var responsibleMock = new Mock<Responsible>();
            responsibleMock.SetupGet(x => x.Plant).Returns(TestPlant);

            var step = new Step(TestPlant, "S", modeMock.Object, responsibleMock.Object);
            step.SetProtectedIdForTesting(StepId);

            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            var project1 = new Project(TestPlant, ProjectNameWithTags, "Desc1");
            var req1 = new TagRequirement(TestPlant, 1, rdMock.Object);
            var req2 = new TagRequirement(TestPlant, 2, rdMock.Object);
            var req3 = new TagRequirement(TestPlant, 4, rdMock.Object);
            _standardTagWith3Reqs = new Tag(TestPlant, TagType.Standard, StandardTagNo, "Desc", step, new List<TagRequirement>
            {
                req1,
                req2,
                req3
            });
            _standardTagWith3Reqs.SetProtectedIdForTesting(StandardTagId);
            project1.AddTag(_standardTagWith3Reqs);
            var req4 = new TagRequirement(TestPlant, 1, rdMock.Object);
            var req5 = new TagRequirement(TestPlant, 2, rdMock.Object);
            var req6 = new TagRequirement(TestPlant, 4, rdMock.Object);
            var poTag = new Tag(TestPlant, TagType.PoArea, PoTagNo, "Desc", step, new List<TagRequirement>
            {
                req4,
                req5,
                req6
            });
            poTag.SetProtectedIdForTesting(PoTagId);
            project1.AddTag(poTag);
            
            var project2 = new Project(TestPlant, ProjectNameWithoutTags, "Desc2");
            
            var projects = new List<Project> {project1, project2};
            var projectsSetMock = projects.AsQueryable().BuildMockDbSet();
            
            ContextHelper
                .ContextMock
                .Setup(x => x.Projects)
                .Returns(projectsSetMock.Object);

            var tags = new List<Tag>{_standardTagWith3Reqs, poTag};
            _tagsSetMock = tags.AsQueryable().BuildMockDbSet();
            
            ContextHelper
                .ContextMock
                .Setup(x => x.Tags)
                .Returns(_tagsSetMock.Object);

            var reqs = new List<TagRequirement>{req1, req2, req3, req4, req5, req6};
            _reqsSetMock = reqs.AsQueryable().BuildMockDbSet();
            
            ContextHelper
                .ContextMock
                .Setup(x => x.TagRequirements)
                .Returns(_reqsSetMock.Object);

            _dut = new ProjectRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetProjectOnlyByNameAsync_KnownProjectWithTags_ShouldReturnProjectWithoutTags()
        {
            var result = await _dut.GetProjectOnlyByNameAsync(ProjectNameWithTags);

            Assert.AreEqual(ProjectNameWithTags, result.Name);
            // Not able to test that Project don't have Tag as children. BuildMockDbSet seem to build Set as a graph with all children
            // Assert.AreEqual(0, result.Tags.Count);
        }

        [TestMethod]
        public async Task GetAllProjectsOnlyAsync_ShouldReturnProjectsWithoutTags()
        {
            var result = await _dut.GetAllProjectsOnlyAsync();

            Assert.AreEqual(2, result.Count);
            // Not able to test that Projects don't have Tag as children. BuildMockDbSet seem to build Set as a graph with all children
            //Assert.IsTrue(result.All(p => p.Tags.Count == 0));
        }

        [TestMethod]
        public async Task GetProjectOnlyByNameAsync_UnknownProject_ShouldReturnNull()
        {
            var result = await _dut.GetProjectOnlyByNameAsync("XYZ");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetTagByTagId_ShouldReturnTag()
        {
            var result = await _dut.GetTagByTagIdAsync(StandardTagId);

            Assert.AreEqual(StandardTagId, result.Id);
        }

        [TestMethod]
        public async Task GetTagsByTagIdsAsync_KnownTag_ShouldReturnTag()
        {
            var result = await _dut.GetTagsByTagIdsAsync(new List<int>{ StandardTagId });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(StandardTagId, result.First().Id);
        }

        [TestMethod]
        public async Task GetTagsByTagIdsAsync_UnknownTag_ShouldReturnEmptyList()
        {
            var result = await _dut.GetTagsByTagIdsAsync(new List<int>{ 9187 });

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetStandardTagsInProjectInStepsAsync_ShouldReturnTags()
        {
            var result = await _dut.GetStandardTagsInProjectInStepsAsync(ProjectNameWithTags, new List<string>{StandardTagNo, PoTagNo}, new List<int>{StepId});

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(StandardTagNo, result.Single().TagNo);
        }

        [TestMethod]
        public async Task GetStandardTagsInProjectOnlyAsync_ShouldReturnTags()
        {
            var result = await _dut.GetStandardTagsInProjectOnlyAsync(ProjectNameWithTags);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(StandardTagNo, result.Single().TagNo);
            // Not able to test that Tags don't have children. BuildMockDbSet seem to build Set as a graph with all children
        }

        [TestMethod]
        public void RemoveTag_ShouldRemoveTagAndRequirementsFromContext()
        {
            // Act
            _dut.RemoveTag(_standardTagWith3Reqs);

            // Assert
            _tagsSetMock.Verify(s => s.Remove(_standardTagWith3Reqs), Times.Once);
            _reqsSetMock.Verify(s => s.Remove(_standardTagWith3Reqs.Requirements.ElementAt(0)), Times.Once);
            _reqsSetMock.Verify(s => s.Remove(_standardTagWith3Reqs.Requirements.ElementAt(1)), Times.Once);
            _reqsSetMock.Verify(s => s.Remove(_standardTagWith3Reqs.Requirements.ElementAt(2)), Times.Once);
        }

        [TestMethod]
        public async Task GetProjectByTagIdAsync_KnownTag_ShouldReturnProjectIncludingTheTag()
        {
            // Act
            var project = await _dut.GetProjectByTagIdAsync(StandardTagId);

            // Assert
            Assert.IsNotNull(project);
            Assert.AreEqual(ProjectNameWithTags, project.Name);
            Assert.IsNotNull(project.Tags);
            var tag = project.Tags.Single(t => t.Id == StandardTagId);
            Assert.IsNotNull(tag);
        }

        [TestMethod]
        public async Task GetProjectByTagIdAsync_UnknownTag_ShouldReturnNull()
        {
            // Act
            var project = await _dut.GetProjectByTagIdAsync(234234);

            // Assert
            Assert.IsNull(project);
        }
    }
}
