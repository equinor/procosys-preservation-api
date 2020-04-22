using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private int _id = 1;
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

            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _stepMock.SetupGet(s => s.Plant).Returns(_oldTitle);
            _stepMock.SetupGet(s => s.Id).Returns(_id);

            journeyRepositoryMock.Setup(s => s.GetStepByStepIdAsync(_id))
                .Returns(Task.FromResult(_stepMock.Object));

            _command = new UpdateStepCommand(_id, _newTitle);

            _dut = new UpdateStepCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateStepCommand_ShouldUpdateStep()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(Unit.Value, result.Data);
            Assert.AreEqual(_stepMock.Object.Title, _newTitle);
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
