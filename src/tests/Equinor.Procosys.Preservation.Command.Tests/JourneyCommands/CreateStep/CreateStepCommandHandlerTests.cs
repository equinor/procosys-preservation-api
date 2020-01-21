using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.CreateStep
{
    [TestClass]
    public class CreateStepCommandHandlerTests
    {
        private const string TestPlant = "TestPlant";
        private const int JourneyId = 1;
        private const int ModeId = 2;
        private const int ResponsibleId = 3;
        private Step _stepAdded;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<Journey> _journeyMock;
        private Mock<Mode> _modeMock;
        private Mock<Responsible> _responsibleMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<IResponsibleRepository> _responsibleRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IPlantProvider> _plantProviderMock;
        private CreateStepCommand _command;
        private CreateStepCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyMock = new Mock<Journey>();
            _journeyRepositoryMock
                .Setup(r => r.GetByIdAsync(JourneyId))
                .Returns(Task.FromResult(_journeyMock.Object));
            _journeyMock
                .Setup(journey => journey.AddStep(It.IsAny<Step>()))
                .Callback<Step>(step =>
                {
                    _stepAdded = step;
                });

            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeMock = new Mock<Mode>();
            _modeMock.SetupGet(x => x.Id).Returns(ModeId);
            _modeRepositoryMock
                .Setup(r => r.GetByIdAsync(ModeId))
                .Returns(Task.FromResult(_modeMock.Object));

            _responsibleRepositoryMock = new Mock<IResponsibleRepository>();
            _responsibleMock = new Mock<Responsible>();
            _responsibleMock.SetupGet(x => x.Id).Returns(ResponsibleId);
            _responsibleRepositoryMock
                .Setup(r => r.GetByIdAsync(ResponsibleId))
                .Returns(Task.FromResult(_responsibleMock.Object));

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock
                .Setup(x => x.Plant)
                .Returns(TestPlant);

            _command = new CreateStepCommand(JourneyId, ModeId, ResponsibleId);

            _dut = new CreateStepCommandHandler(_journeyRepositoryMock.Object,
                _modeRepositoryMock.Object,
                _responsibleRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _plantProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingCreateStepCommand_ShouldAddStepToJourney()
        {
            // Act
            var result = await _dut.Handle(_command, default);
            
            // Assert
            _journeyMock.Verify(u => u.AddStep(It.IsAny<Step>()), Times.Once);
            Assert.AreEqual(ModeId, _stepAdded.ModeId);
            Assert.AreEqual(ResponsibleId, _stepAdded.ResponsibleId);
        }

        [TestMethod]
        public async Task HandlingCreateStepCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
