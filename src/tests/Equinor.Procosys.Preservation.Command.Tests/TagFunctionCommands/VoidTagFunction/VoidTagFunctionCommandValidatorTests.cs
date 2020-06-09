using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagFunctionCommands.VoidTagFunction;
using Equinor.Procosys.Preservation.Command.Validators.TagFunctionValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagFunctionCommands.VoidTagFunction
{
    [TestClass]
    public class VoidTagFunctionCommandValidatorTests
    {
        private Mock<ITagFunctionValidator> _tagFunctionValidatorMock;
        private VoidTagFunctionCommand _command;
        private VoidTagFunctionCommandValidator _dut;

        private readonly string _tagFunctionCode = "TFC";

        [TestInitialize]
        public void Setup_OkState()
        {
            const string RegisterCode = "RC";

            _tagFunctionValidatorMock = new Mock<ITagFunctionValidator>();
            _tagFunctionValidatorMock.Setup(r => r.ExistsAsync(_tagFunctionCode, default)).Returns(Task.FromResult(true));

            _command = new VoidTagFunctionCommand(_tagFunctionCode, RegisterCode, null, Guid.Empty);
            _dut = new VoidTagFunctionCommandValidator(_tagFunctionValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagFunctionNotExists()
        {
            _tagFunctionValidatorMock.Setup(r => r.ExistsAsync(_tagFunctionCode, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag function doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagFunctionIsVoided()
        {
            _tagFunctionValidatorMock.Setup(r => r.IsVoidedAsync(_tagFunctionCode, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag function is already voided!"));
        }
    }
}
