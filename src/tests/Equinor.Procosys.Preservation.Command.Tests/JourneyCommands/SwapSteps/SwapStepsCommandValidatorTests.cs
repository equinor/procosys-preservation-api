using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.SwapSteps;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.SwapSteps
{
    [TestClass]
    public class SwapStepsCommandValidatorTests
    { 
        private SwapStepsCommand _command;
        private SwapStepsCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;

        private int _journeyId = 1;
        private int _stepAId = 2;
        private int _stepBId = 3;
        private readonly string _stepARowVersion = "AAAAAAAAJ00=";
        private readonly string _stepBRowVersion = "AAAAAAAAB00=";
        private readonly string _invalidRowVersion = "String";

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(true));
            _journeyValidatorMock.Setup(r => r.AreAdjacentStepsInAJourneyAsync(_journeyId, _stepAId, _stepBId, default))
                .Returns(Task.FromResult(true));
            _journeyValidatorMock.Setup(r => r.StepExistsAsync(_journeyId, _stepAId, default)).Returns(Task.FromResult(true));
            _journeyValidatorMock.Setup(r => r.StepExistsAsync(_journeyId, _stepBId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_stepARowVersion)).Returns(true);
            _rowVersionValidatorMock.Setup(r => r.IsValid(_stepBRowVersion)).Returns(true);

            _stepValidatorMock = new Mock<IStepValidator>();

            _command = new SwapStepsCommand(_journeyId, _stepAId, _stepARowVersion, _stepBId, _stepBRowVersion);

            _dut = new SwapStepsCommandValidator(_journeyValidatorMock.Object, _stepValidatorMock.Object, _rowVersionValidatorMock.Object);
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
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey doesn't exist!"));
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
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("StepA doesn't exist!"));
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
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("StepB doesn't exist!"));
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

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersionForStepA()
        {
            var command = new SwapStepsCommand(_journeyId, _stepAId, _invalidRowVersion, _stepBId, _stepBRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(_invalidRowVersion)).Returns(false);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid RowVersion for Step A!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersionForStepB()
        {
            var command = new SwapStepsCommand(_journeyId, _stepAId, _stepARowVersion, _stepBId, _invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(_invalidRowVersion)).Returns(false);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid RowVersion for Step B!"));
        }
    }
}
