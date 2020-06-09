using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.CreateJourney
{
    [TestClass]
    public class CreateJourneyCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestJourney = "TestJourney";
        private Journey _journeyAdded;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private CreateJourneyCommand _command;
        private CreateJourneyCommandHandler _dut;
        
        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Journey>()))
                .Callback<Journey>(journey =>
                {
                    _journeyAdded = journey;
                });

            _command = new CreateJourneyCommand(TestJourney, TestUserOid);

            _dut = new CreateJourneyCommandHandler(
                _journeyRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingCreateJourneyCommand_ShouldAddJourneyToRepository()
        {
            // Act
            var result = await _dut.Handle(_command, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual(0, _journeyAdded.Id);
            Assert.AreEqual(TestJourney, _journeyAdded.Title);
            Assert.AreEqual(TestPlant, _journeyAdded.Plant);
        }

        [TestMethod]
        public async Task HandlingCreateJourneyCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(_command.CurrentUserOid, default), Times.Once);
        }
    }
}
