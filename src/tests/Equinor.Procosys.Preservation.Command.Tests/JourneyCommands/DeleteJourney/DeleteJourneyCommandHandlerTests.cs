using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteJourney;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.DeleteJourney
{
    [TestClass]
    public class DeleteJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int JourneyId = 12;
        private const string _rowVersion = "AAAAAAAAABA=";
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Journey _journey;
        private DeleteJourneyCommand _command;
        private DeleteJourneyCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journey = new Journey(TestPlant, "J");
            _journeyRepositoryMock
                .Setup(x => x.GetByIdAsync(JourneyId))
                    .Returns(Task.FromResult(_journey));
            _command = new DeleteJourneyCommand(JourneyId, _rowVersion);

            _dut = new DeleteJourneyCommandHandler(
                _journeyRepositoryMock.Object,
                UnitOfWorkMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingDeleteJourneyCommand_ShouldDeleteJourneyFromRepository()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            _journeyRepositoryMock.Verify(r => r.Remove(_journey), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteJourneyCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
