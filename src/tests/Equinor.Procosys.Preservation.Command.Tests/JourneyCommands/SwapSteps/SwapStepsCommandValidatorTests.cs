using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.SwapSteps
{
    [TestClass]
    public class SwapStepsCommandValidatorTests
    {
        private SwapStepsCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private SwapStepsCommand _command;

        private int _journeyId = 1;
        private int _stepAId = 2;
        private int _stepBId = 3;

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(true));
            _journeyValidatorMock.Setup(r => r.AreAdjacentStepsInAJourneyAsync(_journeyId, _stepAId, _stepBId, default))
                .Returns(Task.FromResult(true));
            _journeyValidatorMock.Setup(r => r.StepExistsAsync(_journeyId, _stepAId, default)).Returns(Task.FromResult(true));
            _journeyValidatorMock.Setup(r => r.StepExistsAsync(_journeyId, _stepBId, default)).Returns(Task.FromResult(true));

            _stepValidatorMock = new Mock<IStepValidator>();

            _command = new SwapStepsCommand(_journeyId, _stepAId, null, _stepBId, null);

            _dut = new SwapStepsCommandValidator(_journeyValidatorMock.Object, _stepValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenJourneyNotExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey does not exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepANotExists()
        {
            // Arrange
            _journeyValidatorMock.Setup(r => r.StepExistsAsync(_journeyId, _stepAId, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("StepA does not exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepBNotExists()
        {
            // Arrange
            _journeyValidatorMock.Setup(r => r.StepExistsAsync(_journeyId, _stepBId, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("StepB does not exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepAAndStepBAreNotAdjacent()
        {
            _journeyValidatorMock.Setup(r => r.AreAdjacentStepsInAJourneyAsync(_journeyId, _stepAId, _stepBId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("StepA and StepB are not adjacent!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepsIncludeSupplierStep()
        {
            _stepValidatorMock.Setup(s => s.IsAnyStepForSupplier(_stepAId, _stepBId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Supplier steps cannot be swapped!"));
        }
    }
}
