﻿using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly int _modeId = 3;
        private readonly int _responsibleId = 4; 
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private readonly string _oldTitle = "StepTitleOld";
        private readonly string _newTitle = "StepTitleNew";
        private Mock<Mode> _modeMock;
        private Mock<Responsible> _responsibleMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<IResponsibleRepository> _responsibleRepositoryMock;

        private UpdateStepCommand _command;
        private UpdateStepCommandHandler _dut;
        private Step _step;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var journeyId = 1;
            var stepId = 2;

            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            var journey = new Journey(TestPlant, "J");
            journey.SetProtectedIdForTesting(journeyId);

            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeMock = new Mock<Mode>();
            _modeMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _modeMock.SetupGet(x => x.Id).Returns(_modeId);
            _modeRepositoryMock
                .Setup(r => r.GetByIdAsync(_modeId))
                .Returns(Task.FromResult(_modeMock.Object));

            _responsibleRepositoryMock = new Mock<IResponsibleRepository>();
            _responsibleMock = new Mock<Responsible>();
            _responsibleMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _responsibleMock.SetupGet(s => s.Id).Returns(_responsibleId);
            _responsibleRepositoryMock
                .Setup(r => r.GetByIdAsync(_responsibleId))
                .Returns(Task.FromResult(_responsibleMock.Object));

            _step = new Step(TestPlant, _oldTitle, _modeMock.Object, _responsibleMock.Object);
            _step.SetProtectedIdForTesting(stepId);
            journey.AddStep(_step);

            journeyRepositoryMock.Setup(s => s.GetByIdAsync(journeyId))
                .Returns(Task.FromResult(journey));
            _command = new UpdateStepCommand(journeyId, stepId, _modeId, _responsibleId, _newTitle, _rowVersion);

            _dut = new UpdateStepCommandHandler(
                journeyRepositoryMock.Object,
                _modeRepositoryMock.Object,
                _responsibleRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateStepCommand_ShouldUpdateStep()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _step.Title);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_newTitle, _step.Title);
            Assert.AreEqual(_modeId, _step.ModeId);
            Assert.AreEqual(_responsibleId, _step.ResponsibleId);
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
        public async Task HandlingUpdateStepCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
