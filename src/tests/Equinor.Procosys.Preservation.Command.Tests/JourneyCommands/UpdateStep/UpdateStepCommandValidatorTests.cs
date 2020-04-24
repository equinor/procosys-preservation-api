using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandValidatorTests
    {
        private UpdateStepCommandValidator _dut;
        private Mock<IStepValidator> _stepValidatorMock;
        private UpdateStepCommand _command;

        private int _stepId = 1;
        private string _title = "Title";

        [TestInitialize]
        public void Setup_OkState()
        {
            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(true));
            _command = new UpdateStepCommand(stepId:_stepId, _title);

            _dut = new UpdateStepCommandValidator(_stepValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepNotExists()
        {
            // Arrange
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_command);

            // Arrange
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSameTitleInJourneyExists()
        {
            // Arrange
            _stepValidatorMock.Setup(s => s.ExistsInJourneyAsync(_stepId, _title, default))
                .Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_command);

            // Arrange
            Assert.IsFalse(result.IsValid);
        }
    }
}
