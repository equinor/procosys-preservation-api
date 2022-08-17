using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagFunctionCommands.UnvoidTagFunction;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagFunctionValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagFunctionCommands.UnvoidTagFunction
{
    [TestClass]
    public class UnvoidTagFunctionCommandValidatorTests
    {
        private Mock<ITagFunctionValidator> _tagFunctionValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UnvoidTagFunctionCommand _command;
        private UnvoidTagFunctionCommandValidator _dut;

        private readonly string _tagFunctionCode = "TFC";
        private readonly string _registerCode = "RC";
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagFunctionValidatorMock = new Mock<ITagFunctionValidator>();
            _tagFunctionValidatorMock.Setup(r => r.ExistsAsync(_tagFunctionCode, _registerCode, default)).Returns(Task.FromResult(true));
            _tagFunctionValidatorMock.Setup(r => r.IsVoidedAsync(_tagFunctionCode, _registerCode, default))
                .Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new UnvoidTagFunctionCommand(_tagFunctionCode, _registerCode, _rowVersion);
            _dut = new UnvoidTagFunctionCommandValidator(_tagFunctionValidatorMock.Object, _rowVersionValidatorMock.Object);
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
        public async Task Validate_ShouldFail_WhenTagFunctionIsNotVoided()
        {
            _tagFunctionValidatorMock.Setup(r => r.IsVoidedAsync(_tagFunctionCode, _registerCode, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag function is not voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new UnvoidTagFunctionCommand(_tagFunctionCode, _registerCode, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
