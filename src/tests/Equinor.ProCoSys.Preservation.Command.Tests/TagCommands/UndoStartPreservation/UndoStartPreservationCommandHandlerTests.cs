using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UndoStartPreservation;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.UndoStartPreservation
{
    [TestClass]
    public class UndoStartPreservationCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string _rowVersion1 = "AAAAAAAAABA=";
        private const string _rowVersion2 = "AAAAAAAABBA=";
        
        private UndoStartPreservationCommand _command;
        private Tag _tag1;
        private Tag _tag2;
        private TagRequirement _req1OnTag1;
        private TagRequirement _req2OnTag1;
        private TagRequirement _req1OnTag2;
        private TagRequirement _req2OnTag2;
        private Mock<RequirementDefinition> _rd1Mock;
        private Mock<RequirementDefinition> _rd2Mock;

        private UndoStartPreservationCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var rdId1 = 17;
            var rdId2 = 18;

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _rd1Mock = new Mock<RequirementDefinition>();
            _rd1Mock.SetupGet(rd => rd.Id).Returns(rdId1);
            _rd1Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);
            _rd2Mock = new Mock<RequirementDefinition>();
            _rd2Mock.SetupGet(rd => rd.Id).Returns(rdId2);
            _rd2Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            var intervalWeeks = 2;
            _req1OnTag1 = new TagRequirement(TestPlant, intervalWeeks, _rd1Mock.Object);
            _req2OnTag1 = new TagRequirement(TestPlant, intervalWeeks, _rd2Mock.Object);
            _req1OnTag2 = new TagRequirement(TestPlant, intervalWeeks, _rd1Mock.Object);
            _req2OnTag2 = new TagRequirement(TestPlant, intervalWeeks, _rd2Mock.Object);

            var tagId1 = 7;
            var tagId2 = 8;
            _tag1 = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement>
            {
                _req1OnTag1, _req2OnTag1
            });
            _tag1.SetProtectedIdForTesting(tagId1);
            _tag2 = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement>
            {
                _req1OnTag2, _req2OnTag2
            });
            _tag2.SetProtectedIdForTesting(tagId2);
            
            _tag1.StartPreservation();
            _tag2.StartPreservation();
            
            var tags = new List<Tag>
            {
                _tag1, _tag2
            };

            var tagIds = new List<int> {tagId1, tagId2};
            var tagIdsWithRowVersion = new List<IdAndRowVersion> {new IdAndRowVersion(tagId1, _rowVersion1), new IdAndRowVersion(tagId2, _rowVersion2)};
            var projectRepoMock = new Mock<IProjectRepository>();
            projectRepoMock.Setup(r => r.GetTagsWithPreservationHistoryByTagIdsAsync(tagIds))
                .Returns(Task.FromResult(tags));
            _command = new UndoStartPreservationCommand(tagIdsWithRowVersion);

            _dut = new UndoStartPreservationCommandHandler(projectRepoMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUndoStartPreservationCommand_ShouldUndoStartPreservationOnAllTags()
        {
            var result = await _dut.Handle(_command, default);

            Assert.AreEqual(0, result.Errors.Count);

            Assert.AreEqual(PreservationStatus.NotStarted, _tag1.Status);
            Assert.AreEqual(PreservationStatus.NotStarted, _tag2.Status);
        }

        [TestMethod]
        public async Task HandlingUndoStartPreservationCommand_ShouldUndoStartPreservationOnAllRequirementsOnAllTags()
        {
            await _dut.Handle(_command, default);

            Assert.IsFalse(_req1OnTag1.NextDueTimeUtc.HasValue);
            Assert.IsFalse(_req2OnTag1.NextDueTimeUtc.HasValue);
            Assert.IsFalse(_req1OnTag2.NextDueTimeUtc.HasValue);
            Assert.IsFalse(_req2OnTag2.NextDueTimeUtc.HasValue);

            Assert.IsFalse(_tag1.NextDueTimeUtc.HasValue);
            Assert.IsFalse(_tag2.NextDueTimeUtc.HasValue);
        }

        [TestMethod]
        public async Task HandlingUndoStartPreservationCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingTransferCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion1, result.Data.First().RowVersion);
            Assert.AreEqual(_rowVersion1, _tag1.RowVersion.ConvertToString());
        }
    }
}
