using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UnvoidStep;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.UnvoidStep
{
    [TestClass]
    public class UnvoidStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private Step _step;
        private Journey _journey;
        private UnvoidStepCommand _command;
        private UnvoidStepCommandHandler _dut;

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
            _step.IsVoided = true;

            _journey.AddStep(_step);

            journeyRepositoryMock
                .Setup(r => r.GetByIdAsync(JourneyId))
                .Returns(Task.FromResult(_journey));

            _command = new UnvoidStepCommand(JourneyId, StepId, _rowVersion);

            _dut = new UnvoidStepCommandHandler(
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUnvoidStepCommand_ShouldUnvoidStep()
        {
            // Arrange
            Assert.IsTrue(_step.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsFalse(_step.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUnvoidStepCommand_ShouldSetRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, _step.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUnvoidStepCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
