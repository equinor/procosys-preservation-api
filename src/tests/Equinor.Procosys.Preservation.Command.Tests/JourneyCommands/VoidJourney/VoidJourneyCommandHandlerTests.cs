using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.VoidJourney;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.VoidJourney
{
    [TestClass]
    public class VoidJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private Journey _journey;
        private VoidJourneyCommand _command;
        private VoidJourneyCommandHandler _dut;
        private Mock<Journey> _journeyMock;

        [TestInitialize]
        public void Setup()
        {
            var journeyId = 2;
            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            _journey = new Journey(TestPlant, "JourneyTitle");
            _journeyMock = new Mock<Journey>(TestPlant, "JourneyTitle");
            _journeyMock.SetupGet(j => j.Plant).Returns(TestPlant);
            _journeyMock.SetupGet(j => j.Id).Returns(journeyId);
            journeyRepositoryMock
                .Setup(r => r.GetByIdAsync(journeyId))
                .Returns(Task.FromResult(_journeyMock.Object));

            _command = new VoidJourneyCommand(journeyId, "AAAAAAAAAAA=");

            _dut = new VoidJourneyCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingVoidJourneyCommand_ShouldVoidJourney()
        {
            // Arrange
            Assert.IsFalse(_journey.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual("AAAAAAAAAAA=", result.Data);
            Assert.IsTrue(_journey.IsVoided);
        }

        [TestMethod]
        public async Task HandlingVoidJourneyCommand_ShouldSetRowVersion()
        {
            await _dut.Handle(_command, default);

            _journeyMock.Verify(u => u.SetRowVersion(_command.RowVersion), Times.Once);
        }

        [TestMethod]
        public async Task HandlingVoidJourneyCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
