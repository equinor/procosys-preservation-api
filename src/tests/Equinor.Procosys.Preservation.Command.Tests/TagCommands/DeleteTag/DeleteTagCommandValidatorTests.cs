using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.DeleteTag;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.DeleteTag
{
    [TestClass]
    public class DeleteTagCommandValidatorTests
    {
        private DeleteTagCommand _command;
        private DeleteTagCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;

        private int _tagId = 1;
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsAsync(_tagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(_tagId, default)).Returns(Task.FromResult(true));
            
            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new DeleteTagCommand(_tagId, _rowVersion);

            _dut = new DeleteTagCommandValidator(
                _tagValidatorMock.Object,
                _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_tagId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(_tagId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is not voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsInUse()
        {
            _tagValidatorMock.Setup(r => r.IsInUseAsync(_tagId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is in use!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new DeleteTagCommand(_tagId, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
