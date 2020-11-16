using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Delete;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.ActionValidators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionAttachmentCommands.Delete
{
    [TestClass]
    public class DeleteActionAttachmentCommandValidatorTests
    {
        private const int _tagId = 1;
        private const int _actionId = 2;
        private const int _attachmentId = 3;
        private readonly string _rowVersion = "AAAAAAAAJ00=";
        private DeleteActionAttachmentCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IActionValidator> _actionValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private DeleteActionAttachmentCommand _command;

        [TestInitialize]
        public void Setup_OkState()
        {
            _projectValidatorMock = new Mock<IProjectValidator>();

            _command = new DeleteActionAttachmentCommand(_tagId, _actionId, _attachmentId, _rowVersion);

            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsActionAttachmentAsync(_tagId, _actionId, _attachmentId, default)).Returns(Task.FromResult(true));

            _actionValidatorMock = new Mock<IActionValidator>();
            _actionValidatorMock.Setup(r => r.ExistsAsync(2, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _dut = new DeleteActionAttachmentCommandValidator(
                _projectValidatorMock.Object,
                _tagValidatorMock.Object,
                _actionValidatorMock.Object,
                _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command); 

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(_command.TagId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(_command.TagId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenActionIsClosed()
        {
            _actionValidatorMock.Setup(r => r.IsClosedAsync(_actionId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Action is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAttachmentNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsActionAttachmentAsync(_tagId, _actionId, _attachmentId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Attachment doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new DeleteActionAttachmentCommand(_tagId, _actionId, _attachmentId, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
