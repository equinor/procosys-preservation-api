using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.VoidStep;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.VoidStep
{
    [TestClass]
    public class VoidStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private Step _step;
        private Journey _journey;
        private VoidStepCommand _command;
        private VoidStepCommandHandler _dut;

        private Mock<Mode> _modeMock;

        private const int JourneyId = 1;
        private const int StepId = 2;
        private const int ModeId = 3;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            _modeMock = new Mock<Mode>();
            _modeMock.SetupGet(m => m.Plant).Returns(TestPlant);
            _modeMock.SetupGet(x => x.Id).Returns(ModeId);

            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            _journey = new Journey(TestPlant, "J");
            _journey.SetProtectedIdForTesting(JourneyId);

            _step = new Step(TestPlant, "S", _modeMock.Object, new Responsible(TestPlant, "RC", "RD"));
            _step.SetProtectedIdForTesting(StepId);

            _journey.AddStep(_step);

            journeyRepositoryMock
                .Setup(r => r.GetByIdAsync(JourneyId))
                .Returns(Task.FromResult(_journey));

            _command = new VoidStepCommand(JourneyId, StepId, _rowVersion);

            _dut = new VoidStepCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingVoidStepCommand_ShouldVoidStep()
        {
            // Arrange
            Assert.IsFalse(_step.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(_step.IsVoided);
        }

        [TestMethod]
        public async Task HandlingVoidStepCommand_ShouldSetRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, _step.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingVoidStepCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
