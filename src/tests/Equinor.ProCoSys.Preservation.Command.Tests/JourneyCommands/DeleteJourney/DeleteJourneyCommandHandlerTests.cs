using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.DeleteJourney;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.DeleteJourney
{
    [TestClass]
    public class DeleteJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int JourneyId = 12;
        private const string RowVersion = "AAAAAAAAABA=";
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
            _command = new DeleteJourneyCommand(JourneyId, RowVersion);

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
        public async Task HandlingDeleteJourneyCommand_ShouldAddDeletedJourneyEvent()
        {
            // Act
            await _dut.Handle(_command, default);
            var eventTypes = _journey.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(DeletedEvent<Journey>));
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
