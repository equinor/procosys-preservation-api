using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.ModeCommands.UnvoidMode;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ModeCommands.UnvoidMode
{
    [TestClass]
    public class UnvoidModeCommandValidatorTests
    {
        private Mock<IModeValidator> _modeValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;

        private UnvoidModeCommand _command;
        private UnvoidModeCommandValidator _dut;
        private readonly int _modeId = 2;
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(true));
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_modeId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new UnvoidModeCommand(_modeId, _rowVersion);
            _dut = new UnvoidModeCommandValidator(_modeValidatorMock.Object, _rowVersionValidatorMock.Object);
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
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenModeIsNotVoided()
        {
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_modeId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is not voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string InvalidRowVersion = "String";

            var command = new UnvoidModeCommand(_modeId, InvalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(InvalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
