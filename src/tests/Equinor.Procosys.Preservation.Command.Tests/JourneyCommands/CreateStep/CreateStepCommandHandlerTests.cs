using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.MainApi.Responsible;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.CreateStep
{
    [TestClass]
    public class CreateStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int JourneyId = 1;
        private const int ModeId = 2;
        private const string ResponsibleCode = "B";
        private const int ResponsibleId = 3;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Journey _journey;
        private Mock<Mode> _modeMock;
        private Mock<Responsible> _responsibleMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<IResponsibleRepository> _responsibleRepositoryMock;
        private Mock<IResponsibleApiService> _responsibleApiServiceMock;
        private CreateStepCommand _command;
        private CreateStepCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journey = new Journey(TestPlant, "J");
            _journeyRepositoryMock
                .Setup(r => r.GetByIdAsync(JourneyId))
                .Returns(Task.FromResult(_journey));

            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeMock = new Mock<Mode>();
            _modeMock.SetupGet(m => m.Plant).Returns(TestPlant);
            _modeMock.SetupGet(x => x.Id).Returns(ModeId);
            _modeRepositoryMock
                .Setup(r => r.GetByIdAsync(ModeId))
                .Returns(Task.FromResult(_modeMock.Object));

            _responsibleRepositoryMock = new Mock<IResponsibleRepository>();
            _responsibleMock = new Mock<Responsible>();
            _responsibleMock.SetupGet(r => r.Id).Returns(ResponsibleId);
            _responsibleMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _responsibleRepositoryMock
                .Setup(r => r.GetByCodeAsync(ResponsibleCode))
                .Returns(Task.FromResult(_responsibleMock.Object));

            _responsibleApiServiceMock = new Mock<IResponsibleApiService>();

            _command = new CreateStepCommand(JourneyId, "S", ModeId, ResponsibleCode);

            _dut = new CreateStepCommandHandler(_journeyRepositoryMock.Object,
                _modeRepositoryMock.Object,
                _responsibleRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _responsibleApiServiceMock.Object);
        }

        [TestMethod]
        public async Task HandlingCreateStepCommand_ShouldAddStepToJourney()
        {
            Assert.AreEqual(0, _journey.Steps.Count);
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            Assert.AreEqual(1, _journey.Steps.Count);
            var stepAdded = _journey.Steps.First();
            Assert.AreEqual("S", stepAdded.Title);
            Assert.AreEqual(ModeId, stepAdded.ModeId);
            Assert.AreEqual(ResponsibleId, stepAdded.ResponsibleId);
        }

        [TestMethod]
        public async Task HandlingCreateStepCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        //TODO: Add more tests
    }
}
