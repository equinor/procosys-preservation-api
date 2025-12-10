using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CompletePreservation;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.CompletePreservation
{
    [TestClass]
    public class CompletePreservationCommandHandlerTests : CommandHandlerTestsBase
    {
        private CompletePreservationCommand _command;
        private Tag _tag1;
        private Tag _tag2;
        private TagRequirement _req1OnTag1;
        private TagRequirement _req2OnTag1;
        private TagRequirement _req1OnTag2;
        private TagRequirement _req2OnTag2;
        private Mock<RequirementDefinition> _rd1Mock;
        private Mock<RequirementDefinition> _rd2Mock;

        private const string RowVersion1 = "AAAAAAAAABA=";
        private const string RowVersion2 = "AAAAAAAABBA=";

        private CompletePreservationCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var rdId1 = 17;
            var rdId2 = 18;
            var tagId1 = 7;
            var tagId2 = 8;
            var stepId1 = 9;
            var stepId2 = 10;

            var step1Mock = new Mock<Step>();
            step1Mock.SetupGet(s => s.Plant).Returns(TestPlant);
            step1Mock.SetupGet(s => s.Id).Returns(stepId1);

            var step2Mock = new Mock<Step>();
            step2Mock.SetupGet(s => s.Plant).Returns(TestPlant);
            step2Mock.SetupGet(s => s.Id).Returns(stepId2);

            var journey = new Journey(TestPlant, "D");
            journey.AddStep(step1Mock.Object);
            journey.AddStep(step2Mock.Object);

            _rd1Mock = new Mock<RequirementDefinition>();
            _rd1Mock.SetupGet(rd => rd.Id).Returns(rdId1);
            _rd1Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);
            _rd2Mock = new Mock<RequirementDefinition>();
            _rd2Mock.SetupGet(rd => rd.Id).Returns(rdId2);
            _rd2Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            _req1OnTag1 = new TagRequirement(TestPlant, 2, _rd1Mock.Object);
            _req2OnTag1 = new TagRequirement(TestPlant, 2, _rd2Mock.Object);
            _req1OnTag2 = new TagRequirement(TestPlant, 2, _rd1Mock.Object);
            _req2OnTag2 = new TagRequirement(TestPlant, 2, _rd2Mock.Object);
            _tag1 = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", step2Mock.Object, new List<TagRequirement>
            {
                _req1OnTag1, _req2OnTag1
            });
            _tag1.StartPreservation();
            _tag1.SetProtectedIdForTesting(tagId1);

            _tag2 = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", step2Mock.Object, new List<TagRequirement>
            {
                _req1OnTag2, _req2OnTag2
            });
            _tag2.StartPreservation();
            _tag2.SetProtectedIdForTesting(tagId2);

            var tags = new List<Tag>
            {
                _tag1, _tag2
            };

            var tagIds = new List<int> { tagId1, tagId2 };
            var tagIdsWithRowVersion = new List<IdAndRowVersion> { new IdAndRowVersion(tagId1, RowVersion1), new IdAndRowVersion(tagId2, RowVersion2) };

            var projectRepoMock = new Mock<IProjectRepository>();
            projectRepoMock.Setup(r => r.GetTagsWithPreservationHistoryByTagIdsAsync(tagIds))
                .Returns(Task.FromResult(tags));

            var journeyRepoMock = new Mock<IJourneyRepository>();
            journeyRepoMock
                .Setup(r => r.GetJourneysByStepIdsAsync(new List<int> { stepId2 }))
                .Returns(Task.FromResult(new List<Journey> { journey }));

            _command = new CompletePreservationCommand(tagIdsWithRowVersion);

            _dut = new CompletePreservationCommandHandler(projectRepoMock.Object, journeyRepoMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingCompletePreservationCommand_ShouldCompletePreservationOnAllTags()
        {
            var result = await _dut.Handle(_command, default);

            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsInstanceOfType(result.Data, typeof(IEnumerable<IdAndRowVersion>));

            Assert.AreEqual(PreservationStatus.Completed, _tag1.Status);
            Assert.AreEqual(PreservationStatus.Completed, _tag2.Status);

            Assert.IsNull(_tag1.NextDueTimeUtc);
            Assert.IsNull(_tag2.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingCompletePreservationCommand_ShouldClearNextDueOnAllRequirementsOnAllTags()
        {
            await _dut.Handle(_command, default);

            Assert.IsNull(_req1OnTag1.NextDueTimeUtc);
            Assert.IsNull(_req2OnTag1.NextDueTimeUtc);
            Assert.IsNull(_req1OnTag2.NextDueTimeUtc);
            Assert.IsNull(_req2OnTag2.NextDueTimeUtc);

            Assert.IsNull(_tag1.NextDueTimeUtc);
            Assert.IsNull(_tag2.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingCompletePreservationCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingCompletePreservationCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(RowVersion1, result.Data.First().RowVersion);
            Assert.AreEqual(RowVersion1, _tag1.RowVersion.ConvertToString());
        }
    }
}
