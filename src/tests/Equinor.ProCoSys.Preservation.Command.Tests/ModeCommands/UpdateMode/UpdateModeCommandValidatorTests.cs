using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.ModeCommands.UpdateMode;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ModeCommands.UpdateMode
{
    [TestClass]
    public class UpdateModeCommandValidatorTests
    {
        private UpdateModeCommandValidator _dut;
        private Mock<IModeValidator> _modeValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UpdateModeCommand _command;

        [TestInitialize]
        public void Setup_OkState()
        {
            _command = new UpdateModeCommand(1, "title", true, "AAAAAAAAJ00=");

            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.ExistsAsync(_command.ModeId, default)).ReturnsAsync(true);

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_command.RowVersion)).Returns(true);

            _dut = new UpdateModeCommandValidator(_modeValidatorMock.Object, _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenModeNotExists()
        {
            _modeValidatorMock.Setup(r => r.ExistsAsync(_command.ModeId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnotherModeWithSameTitleAlreadyExists()
        {
            _modeValidatorMock.Setup(r => r.ExistsAnotherModeWithSameTitleAsync(_command.ModeId, _command.Title, default)).ReturnsAsync(true);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode with title already exists!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenModeIsVoided()
        {
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_command.ModeId, default)).ReturnsAsync(true);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnotherModeForSupplierAlreadyExists_AndUpdatingModeToForSupplier()
        {
            _modeValidatorMock.Setup(r => r.ExistsAnotherModeForSupplierAsync(_command.ModeId, default)).ReturnsAsync(true);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Another mode for supplier already exists!"));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenAnotherModeForSupplierAlreadyExists_AndNotUpdatingModeToForSupplier()
        {
            _modeValidatorMock.Setup(r => r.ExistsAnotherModeForSupplierAsync(_command.ModeId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            _rowVersionValidatorMock.Setup(r => r.IsValid(_command.RowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenUpdatingForSupplierInUsedMode()
        {
            _modeValidatorMock.Setup(r => r.ExistsWithForSupplierValueAsync(_command.ModeId, _command.ForSupplier, default)).ReturnsAsync(false);
            _modeValidatorMock.Setup(r => r.IsUsedInStepAsync(_command.ModeId, default)).ReturnsAsync(true);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Can't change 'For supplier' when mode is used in step(s)!"));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenUpdatingForSupplierInUnusedMode()
        {
            _modeValidatorMock.Setup(r => r.ExistsWithForSupplierValueAsync(_command.ModeId, _command.ForSupplier, default)).ReturnsAsync(false);
            _modeValidatorMock.Setup(r => r.IsUsedInStepAsync(_command.ModeId, default)).ReturnsAsync(false);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenNotUpdatingForSupplierInUsedMode()
        {
            _modeValidatorMock.Setup(r => r.ExistsWithForSupplierValueAsync(_command.ModeId, _command.ForSupplier, default)).ReturnsAsync(true);
            _modeValidatorMock.Setup(r => r.IsUsedInStepAsync(_command.ModeId, default)).ReturnsAsync(true);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }
    }
}
