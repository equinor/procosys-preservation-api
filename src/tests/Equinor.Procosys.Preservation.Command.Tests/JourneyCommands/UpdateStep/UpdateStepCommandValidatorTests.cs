using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using Equinor.Procosys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandValidatorTests
    {
        private UpdateStepCommand _command;
        private UpdateStepCommandValidator _dut;

        private Mock<IJourneyValidator> _journeyValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IModeValidator> _modeValidatorMock;
        private Mock<IResponsibleValidator> _responsibleValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;

        private int _journeyId = 2;
        private int _stepId = 1;
        private int _modeId = 3;
        private string _responsibleCode = "TestCode";
        private string _title = "Title";
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(true));
            _journeyValidatorMock.Setup(r => r.StepExistsAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(true));

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.IsFirstStepOrModeIsNotForSupplier(_journeyId, _modeId, _stepId, default)).Returns(Task.FromResult(true));

            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _responsibleValidatorMock = new Mock<IResponsibleValidator>();

            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _title, false, false, _rowVersion);

            _dut = new UpdateStepCommandValidator(
                _journeyValidatorMock.Object,
                _stepValidatorMock.Object,
                _modeValidatorMock.Object,
                _responsibleValidatorMock.Object,
                _rowVersionValidatorMock.Object);
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
        public void Validate_ShouldFail_WhenStepNotExistsInJourney()
        {
            // Arrange
            _journeyValidatorMock.Setup(r => r.StepExistsAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_command);

            // Arrange
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSameTitleInJourneyExists()
        {
            // Arrange
            _journeyValidatorMock.Setup(r => r.OtherStepExistsWithSameTitleAsync(_journeyId,_stepId, _title, default))
                .Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_command);

            // Arrange
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepIsVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenModeNotExists()
        {
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenModeIsVoided()
        {
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_modeId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenResponsibleExistsAndIsVoided()
        {
            _responsibleValidatorMock.Setup(r => r.ExistsAndIsVoidedAsync(_responsibleCode, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Responsible is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenUpdatingToSupplierStepAndTheStepIsNotTheFirstInTheList()
        {
            _stepValidatorMock.Setup(r => r.IsFirstStepOrModeIsNotForSupplier(_journeyId, _modeId, _stepId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Only the first step can be supplier step!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _title, false, false, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid RowVersion!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSettingBothTransferOnRfccSignAndTransferOnRfocSign()
        {
            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _title, true, true, _rowVersion);

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Both 'Transfer on RFCC signing' and 'Transfer on RFOC signing' can not be set in same step!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSettingTransferOnRfccSign_AndStepExistsWithTransferOnRfccSign()
        {
            _journeyValidatorMock.Setup(r => r.HasOtherStepWithTransferOnRfccSignAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(true));
            
            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _title, true, false, _rowVersion);
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("'Transfer on RFCC signing' can not be set on multiple steps in a journey!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSettingTransferOnRfocSign_AndStepExistsWithTransferOnRfocSign()
        {
            _journeyValidatorMock.Setup(r => r.HasOtherStepWithTransferOnRfocSignAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(true));
            
            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _title, false, true, _rowVersion);
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("'Transfer on RFOC signing' can not be set on multiple steps in a journey!"));
        }
    }
}
