using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidStep;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UnvoidStep
{
    [TestClass]
    public class UnvoidStepCommandValidatorTests
    {
        private UnvoidStepCommandValidator _dut;
        private Mock<IStepValidator> _stepValidatorMock;
        private UnvoidStepCommand _command;

        private int _journeyId = 2;
        private int _stepId = 1;

        [TestInitialize]
        public void Setup_OkState()
        {
            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));

            _command = new UnvoidStepCommand(_journeyId, _stepId, null);
            _dut = new UnvoidStepCommandValidator(_stepValidatorMock.Object);
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
        public void Validate_ShouldFail_WhenStepIsNotVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is not voided!"));
        }
    }
}
