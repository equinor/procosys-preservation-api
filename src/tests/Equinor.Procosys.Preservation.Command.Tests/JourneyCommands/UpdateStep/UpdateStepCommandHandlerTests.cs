using System.Threading.Tasks;
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
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private readonly string _oldTitle = "StepTitleOld";
        private readonly string _newTitle = "StepTitleNew";
        private UpdateStepCommand _command;
        private UpdateStepCommandHandler _dut;
        private Step _step;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(s => s.Plant).Returns(TestPlant);
            var responsibleMock = new Mock<Responsible>();
            responsibleMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var journeyId = 1;
            var stepId = 2;
            _step = new Step(TestPlant, _oldTitle, modeMock.Object, responsibleMock.Object);
            journeyRepositoryMock.Setup(s => s.GetStepByStepIdAsync(stepId))
                .Returns(Task.FromResult(_step));
            _command = new UpdateStepCommand(journeyId, stepId, _newTitle, _rowVersion);

            _dut = new UpdateStepCommandHandler(
                journeyRepositoryMock.Object,
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
