using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Transfer
{
    [TestClass]
    public class TransferCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int Step1OnJourney1Id = 1;
        private const int Step2OnJourney1Id = 2;
        private const int Step1OnJourney2Id = 3;
        private const int Step2OnJourney2Id = 4;

        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private const string RowVersion1 = "AAAAAAAAABA=";
        private const string RowVersion2 = "AAAAAAAABBA=";

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
            _tag1 = new Tag(TestPlant, TagType.Standard, "", "", step1OnJourney1Mock.Object,
                new List<TagRequirement> {reqMock1.Object});
            _tag1.SetProtectedIdForTesting(TagId1);

            var reqMock2 = new Mock<TagRequirement>();
            reqMock2.SetupGet(r => r.Plant).Returns(TestPlant);
            _tag2 = new Tag(TestPlant, TagType.Standard, "", "", step1OnJourney2Mock.Object,
                new List<TagRequirement> {reqMock2.Object});
            _tag2.SetProtectedIdForTesting(TagId2);

            _tag1.StartPreservation();
            _tag2.StartPreservation();

            var projectRepoMock = new Mock<IProjectRepository>();
            
            var tagIds = new List<int> {TagId1, TagId2};
            var tagIdsWithRowVersion = new List<IdAndRowVersion> {new IdAndRowVersion(TagId1, RowVersion1), new IdAndRowVersion(TagId2, RowVersion2)};
            projectRepoMock
                .Setup(r => r.GetTagsByTagIdsAsync(tagIds))
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
            Assert.AreEqual(RowVersion1, result.Data.First().RowVersion);
            Assert.AreEqual(RowVersion1, _tag1.RowVersion.ConvertToString());
        }
    }
}
