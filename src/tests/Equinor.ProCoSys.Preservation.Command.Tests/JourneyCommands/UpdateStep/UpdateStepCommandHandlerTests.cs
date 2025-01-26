﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly int _journeyId = 1;
        private readonly int _stepId = 2;
        private readonly int _modeId = 3;
        private readonly int _responsibleId = 4;
        private readonly int _responsibleId2 = 8;
        private readonly string _responsibleCode = "TestCode";
        private readonly string _responsibleCode2 = "TestCode2";
        private readonly string _rowVersion = "AAAAAAAAABA=";
        private readonly string _oldTitle = "StepTitleOld";
        private readonly string _newTitle = "StepTitleNew";
        private readonly AutoTransferMethod _oldAutoTransferMethod = AutoTransferMethod.OnRfccSign;
        private readonly AutoTransferMethod _newAutoTransferMethod = AutoTransferMethod.OnRfocSign;

        private Responsible _responsible;
        private Responsible _responsible2;
        private Responsible _addedResponsible;
        private Journey _journey;
        private Mock<Mode> _modeMock;
        private Mock<PCSResponsible> _pcsResponsibleMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<IResponsibleRepository> _responsibleRepositoryMock;
        private Mock<IResponsibleApiService> _responsibleApiServiceMock;
        private Mock<IPlantProvider> _plantProviderMock;

        private UpdateStepCommand _command;
        private UpdateStepCommand _commandWithResponsible2;
        private UpdateStepCommandHandler _dut;
        private Step _step;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.Setup(p => p.Plant).Returns(TestPlant);

            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            _journey = new Journey(TestPlant, "J");
            _journey.SetProtectedIdForTesting(_journeyId);

            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeMock = new Mock<Mode>();
            _modeMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _modeMock.SetupGet(x => x.Id).Returns(_modeId);
            _modeRepositoryMock
                .Setup(r => r.GetByIdAsync(_modeId))
                .Returns(Task.FromResult(_modeMock.Object));

            _responsible = new Responsible(TestPlant, _responsibleCode, "D1");
            _responsible.SetProtectedIdForTesting(_responsibleId);
            _responsible2 = new Responsible(TestPlant, _responsibleCode2, "D2");
            _responsible2.SetProtectedIdForTesting(_responsibleId2);

            _responsibleRepositoryMock = new Mock<IResponsibleRepository>();
            _responsibleRepositoryMock
                .Setup(r => r.GetByCodeAsync(_responsibleCode))
                .Returns(Task.FromResult(_responsible));
            _responsibleRepositoryMock
                .Setup(r => r.GetByCodeAsync(_responsibleCode2))
                .Returns(Task.FromResult(_responsible2));
            _responsibleRepositoryMock.Setup(r => r.Add(It.IsAny<Responsible>()))
                .Callback<Responsible>(c => _addedResponsible = c);

            _pcsResponsibleMock = new Mock<PCSResponsible>();

            _responsibleApiServiceMock = new Mock<IResponsibleApiService>();
            _responsibleApiServiceMock.Setup(r => r.TryGetResponsibleAsync(TestPlant, _responsibleCode, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_pcsResponsibleMock.Object));

            _step = new Step(TestPlant, _oldTitle, _modeMock.Object, _responsible)
            {
                AutoTransferMethod = _oldAutoTransferMethod
            };
            _step.SetProtectedIdForTesting(_stepId);
            _journey.AddStep(_step);

            _responsibleApiServiceMock.Setup(s => s.TryGetResponsibleAsync(TestPlant, _responsibleCode, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new PCSResponsible { Description = "ResponsibleTitle" }));

            journeyRepositoryMock.Setup(s => s.GetByIdAsync(_journeyId))
                .Returns(Task.FromResult(_journey));

            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _newTitle, _newAutoTransferMethod, _rowVersion);
            _commandWithResponsible2 = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode2, _newTitle, _newAutoTransferMethod, _rowVersion);

            _dut = new UpdateStepCommandHandler(
                journeyRepositoryMock.Object,
                _modeRepositoryMock.Object,
                _responsibleRepositoryMock.Object,
                UnitOfWorkMock.Object,
                _plantProviderMock.Object,
                _responsibleApiServiceMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateStepCommand_ShouldUpdateStep()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _step.Title);
            Assert.AreEqual(_oldAutoTransferMethod, _step.AutoTransferMethod);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_newTitle, _step.Title);
            Assert.AreEqual(_modeId, _step.ModeId);
            Assert.AreEqual(_newAutoTransferMethod, _step.AutoTransferMethod);
        }

        [TestMethod]
        public async Task HandlingUpdateStepCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _step.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUpdateStepCommand_ShouldFail_WhenResponsibleNotExistsAndCanNotBeCreated()
        {
            // Arrange
            _responsibleRepositoryMock
                .Setup(r => r.GetByCodeAsync(_responsibleCode)).Returns(Task.FromResult((Responsible)null));
            _responsibleApiServiceMock.Setup(s => s.TryGetResponsibleAsync(TestPlant, _responsibleCode, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((PCSResponsible)null));

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual($"Responsible with code {_command.ResponsibleCode} not found", result.Errors[0]);
        }

        [TestMethod]
        public async Task HandlingUpdateStepCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingUpdateStepCommand_ShouldAddResponsibleToRepository_WhenResponsibleNotExists()
        {
            // Arrange 
            _responsibleRepositoryMock
                .Setup(r => r.GetByCodeAsync(_responsibleCode))
                .Returns(Task.FromResult((Responsible)null));

            // Act
            await _dut.Handle(_command, default);

            // Assert
            _responsibleRepositoryMock.Verify(r => r.Add(It.IsAny<Responsible>()), Times.Once);
            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Exactly(2));
            Assert.AreEqual(_addedResponsible.Code, _responsibleCode);

        }

        [TestMethod]
        public async Task HandlingUpdateStepCommand_ShouldNotAddResponsibleToRepository_WhenResponsibleAlreadyExists()
        {
            // Arrange
            Assert.AreEqual(1, _journey.Steps.Count);
            Assert.AreEqual(_responsibleId, _journey.Steps.First().ResponsibleId);

            // Act
            await _dut.Handle(_commandWithResponsible2, default);

            // Assert
            _responsibleRepositoryMock.Verify(r => r.Add(It.IsAny<Responsible>()), Times.Never);
            Assert.AreEqual(_responsibleId2, _journey.Steps.First().ResponsibleId);
            Assert.IsNull(_addedResponsible);
        }
    }
}
