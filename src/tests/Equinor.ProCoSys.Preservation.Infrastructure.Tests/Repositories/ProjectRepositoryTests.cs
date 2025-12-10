using System;
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
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class ProjectRepositoryTests : RepositoryTestBase
    {
        private const string ProjectNameWithTags = "ProjectName1";
        private const string ProjectNameWithoutTags = "ProjectName2";
        private readonly Guid _projectProCoSysGuidWithTags = new Guid("aec8297b-b010-4c5d-91e0-7b1c8664ced8");
        private readonly Guid _projectProCoSysGuidWithoutTags = new Guid("6afabbbf-cf21-4533-93ff-73fe6fdfd27a");
        private Project _project1;
        private const int StepId = 61;
        private const int StandardTagId1 = 71;
        private const int StandardTagId2 = 81;
        private const int PoTagId = 72;
        private const string StandardTagNo1 = "TagNo1";
        private const string StandardTagNo2 = "TagNo3";
        private const string StandardTagNo3 = "TagNo4";
        private readonly Guid _StandardTagGuid1 = new Guid("eb3821c2-bda3-8683-e053-2910000a2633");
        private const string PoTagNo = "TagNo2";
        private const string CommPkg1 = "CommPkg1";
        private const string McPkg1 = "McPkg1";
        private const string CommPkg2 = "CommPkg2";
        private const string McPkg2 = "McPkg2";
        private TagRequirement _standardTag1Requirement1;
        private readonly Action _standardTag3Action = new Action(TestPlant, "T", "D", null);

        private ProjectRepository _dut;
        private Tag _standardTag1With3Reqs;
        private Tag _standardTag2;
        private Tag _standardTag3WithAction;
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

            _project1 = new Project(TestPlant, ProjectNameWithTags, "Desc1", _projectProCoSysGuidWithTags);
            _standardTag1Requirement1 = new TagRequirement(TestPlant, 1, MockRequirementDefinition(1));
            var req2 = new TagRequirement(TestPlant, 2, MockRequirementDefinition(2));
            var req3 = new TagRequirement(TestPlant, 4, MockRequirementDefinition(3));
            _standardTag1With3Reqs = new Tag(
                TestPlant,
                TagType.Standard,
                _StandardTagGuid1,
                StandardTagNo1,
                "Desc",
                step,
                new List<TagRequirement> { _standardTag1Requirement1, req2, req3 })
            {
                CommPkgNo = CommPkg1,
                McPkgNo = McPkg1,
            };

            _standardTag1With3Reqs.SetProtectedIdForTesting(StandardTagId1);
            _project1.AddTag(_standardTag1With3Reqs);

            var reqTag2 = new TagRequirement(TestPlant, 1, MockRequirementDefinition(4));
            _standardTag2 = new Tag(
                TestPlant,
                TagType.Standard,
                Guid.NewGuid(),
                StandardTagNo2,
                "Desc2",
                step,
                new List<TagRequirement> { reqTag2 })
            {
                CommPkgNo = CommPkg2,
                McPkgNo = McPkg2
            };
            _standardTag2.SetProtectedIdForTesting(StandardTagId2);
            _project1.AddTag(_standardTag2);

            var reqTag3 = new TagRequirement(TestPlant, 1, MockRequirementDefinition(5));
            _standardTag3WithAction = new Tag(
                TestPlant,
                TagType.Standard,
                Guid.NewGuid(),
                StandardTagNo3,
                "Desc3",
                step,
                new List<TagRequirement> { reqTag3 })
            {
                CommPkgNo = CommPkg1,
                McPkgNo = McPkg1,
            };

            _standardTag3WithAction.AddAction(_standardTag3Action);

            _project1.AddTag(_standardTag3WithAction);

            var req4 = new TagRequirement(TestPlant, 1, MockRequirementDefinition(6));
            var req5 = new TagRequirement(TestPlant, 2, MockRequirementDefinition(7));
            var req6 = new TagRequirement(TestPlant, 4, MockRequirementDefinition(8));
            var poTag = new Tag(
                TestPlant,
                TagType.PoArea,
                Guid.NewGuid(),
                PoTagNo,
                "Desc",
                step,
                new List<TagRequirement> { req4, req5, req6 });
            poTag.SetProtectedIdForTesting(PoTagId);
            _project1.AddTag(poTag);

            var project2 = new Project(TestPlant, ProjectNameWithoutTags, "Desc2", _projectProCoSysGuidWithoutTags);

            var projects = new List<Project> { _project1, project2 };
            var projectsSetMock = projects.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Projects)
                .Returns(projectsSetMock.Object);

            var tags = new List<Tag> { _standardTag1With3Reqs, poTag };
            _tagsSetMock = tags.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Tags)
                .Returns(_tagsSetMock.Object);

            var reqs = new List<TagRequirement>
            {
                _standardTag1Requirement1,
                req2,
                req3,
                req4,
                req5,
                req6,
                reqTag2
            };
            _reqsSetMock = reqs.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.TagRequirements)
                .Returns(_reqsSetMock.Object);

            _dut = new ProjectRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetProjectOnlyByTagGuidAsync_ExistingTag_ShouldReturnProjectWithoutTags()
        {
            var result = await _dut.GetProjectOnlyByTagGuidAsync(_StandardTagGuid1);

            Assert.AreEqual(ProjectNameWithTags, result.Name);
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
        public async Task GetProjectOnlyByNameAsync_UnknownProject_ShouldReturnNull()
        {
            var result = await _dut.GetProjectOnlyByNameAsync("XYZ");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetTagWithActionsByTagId_ShouldReturnTag()
        {
            var result = await _dut.GetTagWithActionsByTagIdAsync(StandardTagId1);

            Assert.AreEqual(StandardTagId1, result.Id);
        }

        [TestMethod]
        public async Task GetTagWithAttachmentsByTagId_ShouldReturnTag()
        {
            var result = await _dut.GetTagWithAttachmentsByTagIdAsync(StandardTagId1);

            Assert.AreEqual(StandardTagId1, result.Id);
        }

        [TestMethod]
        public async Task GetTagOnlyByTagId_ShouldReturnTag()
        {
            var result = await _dut.GetTagOnlyByTagIdAsync(StandardTagId1);

            Assert.AreEqual(StandardTagId1, result.Id);
        }

        [TestMethod]
        public async Task GetTagOnlyByGuid_ShouldReturnTag()
        {
            var result = await _dut.GetTagOnlyByGuidAsync(_standardTag1With3Reqs.Guid);

            Assert.AreEqual(_standardTag1With3Reqs.Id, result.Id);
        }

        [TestMethod]
        public async Task GetTagOnlyByGuid_UnknownGuid_ShouldReturnNull()
        {
            var result = await _dut.GetTagOnlyByGuidAsync(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetTagWithPreservationHistoryByTagId_ShouldReturnTag()
        {
            var result = await _dut.GetTagWithPreservationHistoryByTagIdAsync(StandardTagId1);

            Assert.AreEqual(StandardTagId1, result.Id);
        }

        [TestMethod]
        public async Task GetTagsOnlyByTagIdsAsync_KnownTag_ShouldReturnTag()
        {
            var result = await _dut.GetTagsOnlyByTagIdsAsync(new List<int> { StandardTagId1 });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(StandardTagId1, result.First().Id);
        }

        [TestMethod]
        public async Task GetTagsOnlyByTagIdsAsync_UnknownTag_ShouldReturnEmptyList()
        {
            var result = await _dut.GetTagsOnlyByTagIdsAsync(new List<int> { 9187 });

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetTagsWithPreservationHistoryByTagIdsAsync_KnownTag_ShouldReturnTag()
        {
            var result = await _dut.GetTagsWithPreservationHistoryByTagIdsAsync(new List<int> { StandardTagId1 });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(StandardTagId1, result.First().Id);
        }

        [TestMethod]
        public async Task GetTagsWithPreservationHistoryByTagIdsAsync_UnknownTag_ShouldReturnEmptyList()
        {
            var result = await _dut.GetTagsWithPreservationHistoryByTagIdsAsync(new List<int> { 9187 });

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetStandardTagsInProjectInStepsAsync_ShouldReturnTags()
        {
            var result = await _dut.GetStandardTagsInProjectInStepsAsync(ProjectNameWithTags,
                new List<string> { StandardTagNo1, PoTagNo }, new List<int> { StepId });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(StandardTagNo1, result.Single().TagNo);
        }

        [TestMethod]
        public async Task GetStandardTagsInProjectOnlyAsync_ShouldReturnTags()
        {
            var result = await _dut.GetStandardTagsInProjectOnlyAsync(ProjectNameWithTags);

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Contains(_standardTag1With3Reqs));
            // Not able to test that Tags don't have children. BuildMockDbSet seem to build Set as a graph with all children
        }

        [TestMethod]
        public void RemoveTag_ShouldRemoveTagAndRequirementsFromContext()
        {
            // Act
            _dut.RemoveTag(_standardTag1With3Reqs);

            // Assert
            _tagsSetMock.Verify(s => s.Remove(_standardTag1With3Reqs), Times.Once);
            _reqsSetMock.Verify(s => s.Remove(_standardTag1With3Reqs.Requirements.ElementAt(0)), Times.Once);
            _reqsSetMock.Verify(s => s.Remove(_standardTag1With3Reqs.Requirements.ElementAt(1)), Times.Once);
            _reqsSetMock.Verify(s => s.Remove(_standardTag1With3Reqs.Requirements.ElementAt(2)), Times.Once);
        }

        [TestMethod]
        public async Task GetProjectAndTagWithPreservationHistoryByTagIdAsync_KnownTag_ShouldReturnProjectIncludingTheTag()
        {
            // Act
            var project = await _dut.GetProjectAndTagWithPreservationHistoryByTagIdAsync(StandardTagId1);

            // Assert
            Assert.IsNotNull(project);
            Assert.AreEqual(ProjectNameWithTags, project.Name);
            Assert.AreEqual(_projectProCoSysGuidWithTags, project.Guid);
            Assert.IsNotNull(project.Tags);
            var tag = project.Tags.Single(t => t.Id == StandardTagId1);
            Assert.IsNotNull(tag);
        }

        [TestMethod]
        public async Task GetProjectAndTagWithPreservationHistoryByTagIdAsync_UnknownTag_ShouldReturnNull()
        {
            // Act
            var project = await _dut.GetProjectAndTagWithPreservationHistoryByTagIdAsync(234234);

            // Assert
            Assert.IsNull(project);
        }

        [TestMethod]
        public async Task GetTagByActionGuidAsync_ShouldReturnTag()
        {
            // Acct
            var result = await _dut.GetTagByActionGuidAsync(_standardTag3Action.Guid);

            // Assert
            Assert.AreEqual(_standardTag3WithAction, result);
        }

        [TestMethod]
        public async Task GetTagByTagRequirementGuidAsync_ShouldReturnTag()
        {
            // Acct
            var result = await _dut.GetTagByTagRequirementGuidAsync(_standardTag1Requirement1.Guid);

            // Assert
            Assert.AreEqual(_standardTag1With3Reqs, result);
        }

        private RequirementDefinition MockRequirementDefinition(int sortKey)
        {
            var requirementDefinition = new Mock<RequirementDefinition>();
            requirementDefinition.SetupGet(rd => rd.Plant).Returns(TestPlant);
            requirementDefinition.SetupGet(rd => rd.Id).Returns(sortKey);

            return requirementDefinition.Object;
        }
    }
}
