using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.ModeCommands.DeleteMode;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ModeCommands.DeleteMode
{
    [TestClass]
    public class DeleteModeCommandValidatorTests
    {
        private DeleteModeCommandValidator _dut;
        private Mock<IModeValidator> _modeValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private DeleteModeCommand _command;

        private int _id = 1;
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(true));
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_id, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new DeleteModeCommand(_id, _rowVersion);

            _dut = new DeleteModeCommandValidator(_modeValidatorMock.Object, _rowVersionValidatorMock.Object);
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
            _modeValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenModeNotVoided()
        {
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_id, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is not voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenModeIsUsedInAStep()
        {
            _modeValidatorMock.Setup(r => r.IsUsedInStepAsync(_id, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is used in step(s)!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string InvalidRowVersion = "String";

            var command = new DeleteModeCommand(_id, InvalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(InvalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
