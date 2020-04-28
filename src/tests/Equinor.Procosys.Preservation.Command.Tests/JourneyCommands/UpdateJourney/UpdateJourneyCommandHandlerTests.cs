using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateJourney
{
    [TestClass]
    public class UpdateJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _oldTitle = "JourneyTitleOld";
        private readonly string _newTitle = "JourneyTitleNew";

        private UpdateJourneyCommand _command;
        private UpdateJourneyCommandHandler _dut;
        private Mock<Journey> _journeyMock;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var testJourneyId = 1;
            var journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyMock = new Mock<Journey>(TestPlant, _oldTitle);
            _journeyMock.SetupGet(j => j.Plant).Returns(TestPlant);
            _journeyMock.SetupGet(j => j.Id).Returns(testJourneyId);
            journeyRepositoryMock.Setup(j => j.GetByIdAsync(testJourneyId))
                .Returns(Task.FromResult(_journeyMock.Object));
            _command = new UpdateJourneyCommand(testJourneyId, _newTitle);

            _dut = new UpdateJourneyCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldUpdateJourney()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _journeyMock.Object.Title);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(Unit.Value, result.Data);
            Assert.AreEqual(_journeyMock.Object.Title, _newTitle);
        }

        [TestMethod]
        public async Task HandlingUpdateJourneyCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
