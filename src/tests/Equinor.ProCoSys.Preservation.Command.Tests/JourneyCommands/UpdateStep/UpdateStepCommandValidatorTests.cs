using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.UpdateStep
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
            _journeyValidatorMock.Setup(r => r.ExistsStepAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(true));

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.HasModeAsync(_modeId, _stepId, default)).Returns(Task.FromResult(true));

            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _responsibleValidatorMock = new Mock<IResponsibleValidator>();

            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _title, AutoTransferMethod.None, _rowVersion);

            _dut = new UpdateStepCommandValidator(
                _journeyValidatorMock.Object,
                _stepValidatorMock.Object,
                _modeValidatorMock.Object,
                _responsibleValidatorMock.Object,
                _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenStepNotExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsStepAsync(_journeyId, _stepId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey and/or step doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenSameTitleInJourneyExists()
        {
            // Arrange
            _journeyValidatorMock.Setup(r => r.OtherStepExistsWithSameTitleAsync(_journeyId, _stepId, _title, default))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _dut.ValidateAsync(_command);

            // Arrange
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenStepIsVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenModeNotExists()
        {
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenChangeToAVoidedMode()
        {
            _stepValidatorMock.Setup(r => r.HasModeAsync(_modeId, _stepId, default)).Returns(Task.FromResult(false));
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_modeId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenExistingModeIsVoided()
        {
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_modeId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenResponsibleExistsAndIsVoided()
        {
            _responsibleValidatorMock.Setup(r => r.ExistsAndIsVoidedAsync(_responsibleCode, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Responsible is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _title, AutoTransferMethod.None, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenSettingNoneAutoTransferMethod_AndAExistingStepHasNone()
        {
            var autoTransferMethod = AutoTransferMethod.None;
            _journeyValidatorMock.Setup(r => r.HasOtherStepWithAutoTransferMethodAsync(_journeyId, _stepId, autoTransferMethod, default)).Returns(Task.FromResult(true));

            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _title, autoTransferMethod, _rowVersion);
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }
    }
}
