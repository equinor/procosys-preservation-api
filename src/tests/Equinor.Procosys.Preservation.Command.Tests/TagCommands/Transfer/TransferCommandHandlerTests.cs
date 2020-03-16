using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
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

        private TransferCommand _command;

        private TransferCommandHandler _dut;
        private Mock<Tag> _tag1Mock;
        private Mock<Tag> _tag2Mock;

        [TestInitialize]
        public void Setup()
        {
            var step1OnJourney1Mock = new Mock<Step>();
            step1OnJourney1Mock.SetupGet(x => x.Id).Returns(Step1OnJourney1Id);
            step1OnJourney1Mock.SetupGet(x => x.Schema).Returns(TestPlant);
            var step2OnJourney1Mock = new Mock<Step>();
            step2OnJourney1Mock.SetupGet(x => x.Id).Returns(Step2OnJourney1Id);
            step2OnJourney1Mock.SetupGet(x => x.Schema).Returns(TestPlant);

            var journey1 = new Journey(TestPlant,"");
            journey1.AddStep(step1OnJourney1Mock.Object);
            journey1.AddStep(step2OnJourney1Mock.Object);

            var step1OnJourney2Mock = new Mock<Step>();
            step1OnJourney2Mock.SetupGet(x => x.Id).Returns(Step1OnJourney2Id);
            step1OnJourney2Mock.SetupGet(x => x.Schema).Returns(TestPlant);
            var step2OnJourney2Mock = new Mock<Step>();
            step2OnJourney2Mock.SetupGet(x => x.Id).Returns(Step2OnJourney2Id);
            step2OnJourney2Mock.SetupGet(x => x.Schema).Returns(TestPlant);

            var journey2 = new Journey(TestPlant,"");
            journey2.AddStep(step1OnJourney2Mock.Object);
            journey2.AddStep(step2OnJourney2Mock.Object);

            var journeyRepoMock = new Mock<IJourneyRepository>();
            journeyRepoMock
                .Setup(r => r.GetJourneysByStepIdsAsync(new List<int> {Step1OnJourney1Id, Step1OnJourney2Id}))
                .Returns(Task.FromResult(new List<Journey> {journey1, journey2}));

            var reqMock1 = new Mock<Requirement>();
            reqMock1.SetupGet(r => r.Schema).Returns(TestPlant);
            _tag1Mock = new Mock<Tag>(TestPlant, TagType.Standard, "", "", "", "", "", "", "", "", "", "", "", step1OnJourney1Mock.Object,
                new List<Requirement> {reqMock1.Object});
            _tag1Mock.SetupGet(t => t.Id).Returns(TagId1);
            _tag1Mock.SetupGet(t => t.Schema).Returns(TestPlant);

            var reqMock2 = new Mock<Requirement>();
            reqMock2.SetupGet(r => r.Schema).Returns(TestPlant);
            _tag2Mock = new Mock<Tag>(TestPlant, TagType.Standard, "", "", "", "", "", "", "", "", "", "", "", step1OnJourney2Mock.Object,
                new List<Requirement> {reqMock2.Object});
            _tag2Mock.SetupGet(t => t.Id).Returns(TagId2);
            _tag2Mock.SetupGet(t => t.Schema).Returns(TestPlant);

            _tag1Mock.Object.StartPreservation();
            _tag2Mock.Object.StartPreservation();

            var projectRepoMock = new Mock<IProjectRepository>();
            
            var tagIds = new List<int> {TagId1, TagId2};
            projectRepoMock
                .Setup(r => r.GetTagsByTagIdsAsync(tagIds))
                .Returns(Task.FromResult(new List<Tag> {_tag1Mock.Object, _tag2Mock.Object}));

            _command = new TransferCommand(tagIds);

            _dut = new TransferCommandHandler(projectRepoMock.Object, journeyRepoMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingTransferCommand_ShouldTransferToNextStep()
        {
            await _dut.Handle(_command, default);

            Assert.AreEqual(Step2OnJourney1Id, _tag1Mock.Object.StepId);
            Assert.AreEqual(Step2OnJourney2Id, _tag2Mock.Object.StepId);
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
    }
}
