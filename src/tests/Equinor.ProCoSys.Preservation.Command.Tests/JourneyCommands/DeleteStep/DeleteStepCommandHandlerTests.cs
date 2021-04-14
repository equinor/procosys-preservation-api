using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.DeleteStep;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.DeleteStep
{
    [TestClass]
    public class DeleteStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private int _journeyId = 1;
        private int _stepId = 2;
        private const string _rowVersion = "AAAAAAAAABA=";
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private DeleteStepCommand _command;
        private DeleteStepCommandHandler _dut;
        private Mock<Step> _stepMock;
        private Journey _journey;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journey = new Journey(TestPlant, "J");
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _stepMock.SetupGet(s => s.Id).Returns(_stepId);
            _stepMock.Object.IsVoided = true;
            _journey.AddStep(_stepMock.Object);

            _journeyRepositoryMock
                .Setup(x => x.GetByIdAsync(_journeyId))
                    .Returns(Task.FromResult(_journey));
            _command = new DeleteStepCommand(_journeyId, _stepId, _rowVersion);

            _dut = new DeleteStepCommandHandler(_journeyRepositoryMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingDeleteStepCommand_ShouldDeleteStepFromJourney()
        {
            // Arrange
            Assert.AreEqual(1, _journey.Steps.Count);
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            Assert.AreEqual(0, _journey.Steps.Count);
        }

        [TestMethod]
        public async Task HandlingDeleteStepCommand_ShouldDeleteStepFromRepo()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            _journeyRepositoryMock.Verify(r => r.RemoveStep(_stepMock.Object), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteStepCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
