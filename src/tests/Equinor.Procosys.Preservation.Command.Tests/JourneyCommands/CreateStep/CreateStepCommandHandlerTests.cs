using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.CreateStep
{
    [TestClass]
    public class CreateStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int JourneyId = 1;
        private const int ModeId = 2;
        private const string ResponsibleCode = "B";
        private const int ResponsibleId = 3;
        private readonly string _title = "S";
        private readonly AutoTransferMethod _autoTransferMethod = AutoTransferMethod.OnRfccSign;

        private Responsible _addedResponsible;
        private Journey _journey;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<Mode> _modeMock;
        private Responsible _responsible;
        private Mock<ProcosysResponsible> _pcsResponsibleMock;
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
            _responsible = new Responsible(TestPlant, ResponsibleCode, "T");
            _responsible.SetProtectedIdForTesting(ResponsibleId);
            _responsibleRepositoryMock
                .Setup(r => r.GetByCodeAsync(ResponsibleCode))
                .Returns(Task.FromResult(_responsible));
            _responsibleRepositoryMock.Setup(r => r.Add(It.IsAny<Responsible>()))
                .Callback<Responsible>(c => _addedResponsible = c);

            _pcsResponsibleMock = new Mock<ProcosysResponsible>();

            _responsibleApiServiceMock = new Mock<IResponsibleApiService>();
            _responsibleApiServiceMock.Setup(r => r.TryGetResponsibleAsync(TestPlant, ResponsibleCode))
                .Returns(Task.FromResult(_pcsResponsibleMock.Object));

            _command = new CreateStepCommand(JourneyId, _title, ModeId, ResponsibleCode, _autoTransferMethod);

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
            Assert.AreEqual(_title, stepAdded.Title);
            Assert.AreEqual(_autoTransferMethod, stepAdded.AutoTransferMethod);
            Assert.AreEqual(ModeId, stepAdded.ModeId);
            Assert.AreEqual(ResponsibleId, stepAdded.ResponsibleId);
        }

        [TestMethod]
        public async Task HandlingCreateStepCommand_ShouldFail_WhenResponsibleNotExistsAndCanNotBeCreated()
        {
            // Arrange
            _responsibleRepositoryMock
                .Setup(r => r.GetByCodeAsync(ResponsibleCode)).Returns(Task.FromResult((Responsible)null));
            _responsibleApiServiceMock.Setup(s => s.TryGetResponsibleAsync(TestPlant, ResponsibleCode))
                .Returns(Task.FromResult((ProcosysResponsible)null));

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert

            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual($"Responsible with code {_command.ResponsibleCode} not found", result.Errors[0]);
        }

        [TestMethod]
        public async Task HandlingCreateStepCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingCreateStepCommand_ShouldAddResponsibleToRepository_WhenResponsibleNotExists()
        {
            // Arrange 
            _responsibleRepositoryMock
                .Setup(r => r.GetByCodeAsync(ResponsibleCode))
                .Returns(Task.FromResult((Responsible)null));

            // Act
            await _dut.Handle(_command, default);

            // Assert
            _responsibleRepositoryMock.Verify(r => r.Add(It.IsAny<Responsible>()), Times.Once);
            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Exactly(2));
            Assert.AreEqual(_addedResponsible.Code, ResponsibleCode);
        }

        [TestMethod]
        public async Task HandlingCreateStepCommand_ShouldNotAddResponsibleToRepository_WhenResponsibleAlreadyExists()
        {
            Assert.AreEqual(0, _journey.Steps.Count);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            _responsibleRepositoryMock.Verify(r => r.Add(It.IsAny<Responsible>()), Times.Never);
            Assert.AreEqual(ResponsibleId, _journey.Steps.First().ResponsibleId);
            Assert.IsNull(_addedResponsible);
        }
    }
}
