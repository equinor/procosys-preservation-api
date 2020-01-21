using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.CreateJourney
{
    [TestClass]
    public class CreateJourneyCommandHandlerTests
    {
        private const string TestPlant = "TestPlant";
        private const string TestJourney = "TestJourney";
        private Journey _journeyAdded;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IPlantProvider> _plantProviderMock;
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
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock
                .Setup(x => x.Plant)
                .Returns(TestPlant);

            _command = new CreateJourneyCommand(TestJourney);

            _dut = new CreateJourneyCommandHandler(
                _journeyRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _plantProviderMock.Object);
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
            Assert.AreEqual(TestPlant, _journeyAdded.Schema);
        }

        [TestMethod]
        public async Task HandlingCreateJourneyCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
