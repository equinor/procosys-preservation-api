using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteStep;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.DeleteStep
{
    [TestClass]
    public class DeleteStepCommandValidatorTests
    {
        private DeleteStepCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private DeleteStepCommand _command;

        private int _journeyId = 1;
        private int _stepId = 2;
        private readonly string _rowVersion = "AAAAAAAAJ00=";
        private Mock<IStepValidator> _stepValidatorMock;

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsStepAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));
            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new DeleteStepCommand(_journeyId, _stepId, _rowVersion);

            _dut = new DeleteStepCommandValidator(
                _journeyValidatorMock.Object,
                _stepValidatorMock.Object,
                _rowVersionValidatorMock.Object);
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
            _journeyValidatorMock.Setup(r => r.ExistsStepAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey and/or step doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepNotVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is not voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyStepInJourneyIsInUse()
        {
            _journeyValidatorMock.Setup(r => r.IsAnyStepInJourneyInUseAsync(_journeyId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("No steps can be deleted from journey when preservation tags exists in journey!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new DeleteStepCommand(_journeyId, _stepId, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
