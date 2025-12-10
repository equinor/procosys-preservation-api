using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagFunctionCommands.VoidTagFunction;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagFunctionValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagFunctionCommands.VoidTagFunction
{
    [TestClass]
    public class VoidTagFunctionCommandValidatorTests
    {
        private Mock<ITagFunctionValidator> _tagFunctionValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private VoidTagFunctionCommand _command;
        private VoidTagFunctionCommandValidator _dut;

        private readonly string _tagFunctionCode = "TFC";
        private readonly string _rowVersion = "AAAAAAAAJ00=";
        private readonly string _registerCode = "RC";

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagFunctionValidatorMock = new Mock<ITagFunctionValidator>();
            _tagFunctionValidatorMock.Setup(r => r.ExistsAsync(_tagFunctionCode, _registerCode, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new VoidTagFunctionCommand(_tagFunctionCode, _registerCode, _rowVersion);
            _dut = new VoidTagFunctionCommandValidator(_tagFunctionValidatorMock.Object, _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagFunctionNotExists()
        {
            _tagFunctionValidatorMock.Setup(r => r.ExistsAsync(_tagFunctionCode, _registerCode, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag function doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagFunctionIsVoided()
        {
            _tagFunctionValidatorMock.Setup(r => r.IsVoidedAsync(_tagFunctionCode, _registerCode, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag function is already voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string InvalidRowVersion = "String";

            var command = new VoidTagFunctionCommand(_tagFunctionCode, _registerCode, InvalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(InvalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
