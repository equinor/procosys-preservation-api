using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.Transfer;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.Transfer
{
    [TestClass]
    public class TransferCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int Step1OnJourney1Id = 1;
        private const int Step2OnJourney1Id = 2;
        private const int Step1OnJourney2Id = 3;
        private const int Step2OnJourney2Id = 4;

        private const string _rowVersion1 = "AAAAAAAAABA=";
        private const string _rowVersion2 = "AAAAAAAABBA=";

        private TransferCommand _command;

        private TransferCommandHandler _dut;
        private Tag _tag1;
        private Tag _tag2;

        [TestInitialize]
        public void Setup()
        {
            var step1OnJourney1Mock = new Mock<Step>();
            step1OnJourney1Mock.SetupGet(x => x.Id).Returns(Step1OnJourney1Id);
            step1OnJourney1Mock.SetupGet(x => x.Plant).Returns(TestPlant);
            var step2OnJourney1Mock = new Mock<Step>();
            step2OnJourney1Mock.SetupGet(x => x.Id).Returns(Step2OnJourney1Id);
            step2OnJourney1Mock.SetupGet(x => x.Plant).Returns(TestPlant);

            var journey1 = new Journey(TestPlant, "J1");
            journey1.AddStep(step1OnJourney1Mock.Object);
            journey1.AddStep(step2OnJourney1Mock.Object);

            var step1OnJourney2Mock = new Mock<Step>();
            step1OnJourney2Mock.SetupGet(x => x.Id).Returns(Step1OnJourney2Id);
            step1OnJourney2Mock.SetupGet(x => x.Plant).Returns(TestPlant);
            var step2OnJourney2Mock = new Mock<Step>();
            step2OnJourney2Mock.SetupGet(x => x.Id).Returns(Step2OnJourney2Id);
            step2OnJourney2Mock.SetupGet(x => x.Plant).Returns(TestPlant);

            var journey2 = new Journey(TestPlant, "J2");
            journey2.AddStep(step1OnJourney2Mock.Object);
            journey2.AddStep(step2OnJourney2Mock.Object);

            var journeyRepoMock = new Mock<IJourneyRepository>();
            journeyRepoMock
                .Setup(r => r.GetJourneysByStepIdsAsync(new List<int> {Step1OnJourney1Id, Step1OnJourney2Id}))
                .Returns(Task.FromResult(new List<Journey> {journey1, journey2}));

            var reqMock1 = new Mock<TagRequirement>();
            reqMock1.SetupGet(r => r.Plant).Returns(TestPlant);
            
            var tagId1 = 7;
            var tagId2 = 8;
            _tag1 = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", step1OnJourney1Mock.Object,
                new List<TagRequirement> {reqMock1.Object});
            _tag1.SetProtectedIdForTesting(tagId1);

            var reqMock2 = new Mock<TagRequirement>();
            reqMock2.SetupGet(r => r.Plant).Returns(TestPlant);
            _tag2 = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", step1OnJourney2Mock.Object,
                new List<TagRequirement> {reqMock2.Object});
            _tag2.SetProtectedIdForTesting(tagId2);

            _tag1.StartPreservation();
            _tag2.StartPreservation();

            var projectRepoMock = new Mock<IProjectRepository>();
            
            var tagIds = new List<int> {tagId1, tagId2};
            var tagIdsWithRowVersion = new List<IdAndRowVersion> {new IdAndRowVersion(tagId1, _rowVersion1), new IdAndRowVersion(tagId2, _rowVersion2)};
            projectRepoMock
                .Setup(r => r.GetTagsOnlyByTagIdsAsync(tagIds))
                .Returns(Task.FromResult(new List<Tag> {_tag1, _tag2}));

            _command = new TransferCommand(tagIdsWithRowVersion);

            _dut = new TransferCommandHandler(projectRepoMock.Object, journeyRepoMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingTransferCommand_ShouldTransferToNextStep()
        {
            await _dut.Handle(_command, default);

            Assert.AreEqual(Step2OnJourney1Id, _tag1.StepId);
            Assert.AreEqual(Step2OnJourney2Id, _tag2.StepId);
        }

        [TestMethod]
        public async Task HandlingTransferCommand_ShouldThrowException_WhenTransferFromLastStep()
        {
            // transfer to last step
            await _dut.Handle(_command, default);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _dut.Handle(_command, default)
            );
        }

        [TestMethod]
        public async Task HandlingTransferCommand_ShouldSave()
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
