using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidJourney;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UnvoidJourney
{
    [TestClass]
    public class UnvoidJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private Journey _journey;
        private UnvoidJourneyCommand _command;
        private UnvoidJourneyCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var journeyId = 2;
            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            _journey = new Journey(TestPlant, "Test Journey");

            journeyRepositoryMock
                .Setup(r => r.GetByIdAsync(journeyId))
                .Returns(Task.FromResult(_journey));

            _command = new UnvoidJourneyCommand(journeyId);

            _dut = new UnvoidJourneyCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object); 
        }

        [TestMethod]
        public async Task HandlingUnvoidJourneyCommand_ShouldUnvoidJourney()
        {
            // Arrange
            Assert.IsFalse(_journey.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsFalse(_journey.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUnvoidJourneyCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
