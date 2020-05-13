using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly int _id = 1;

        private readonly string _oldTitle = "StepTitleOld";
        private readonly string _newTitle = "StepTitleNew";
        private UpdateStepCommand _command;
        private UpdateStepCommandHandler _dut;
        private Mock<Step> _stepMock;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(s => s.Plant).Returns(TestPlant);
            var responsibleMock = new Mock<Responsible>();
            responsibleMock.SetupGet(s => s.Plant).Returns(TestPlant);

            _stepMock = new Mock<Step>(TestPlant, _oldTitle, modeMock.Object, responsibleMock.Object);
            _stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _stepMock.SetupGet(s => s.Id).Returns(_id);
            journeyRepositoryMock.Setup(s => s.GetStepByStepIdAsync(_stepMock.Object.Id))
                .Returns(Task.FromResult(_stepMock.Object));
            _command = new UpdateStepCommand(_id, _newTitle, null);

            _dut = new UpdateStepCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateStepCommand_ShouldUpdateStep()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _stepMock.Object.Title);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual("AAAAAAAAAAA=", result.Data);
            Assert.AreEqual(_newTitle, _stepMock.Object.Title);
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
        public async Task HandlingUpdateJourneyCommand_ShouldSetRowVersion()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            _stepMock.Verify(u => u.SetRowVersion(_command.RowVersion), Times.Once);
        }
    }
}
