using System.IO;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.ActionAttachmentCommands.Upload;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ActionValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ActionAttachmentCommands.Upload
{
    [TestClass]
    public class UploadActionAttachmentCommandValidatorTests
    {
        private UploadActionAttachmentCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IActionValidator> _actionValidatorMock;
        private UploadActionAttachmentCommand _commandWithoutOverwrite;

        private readonly int _tagId = 2;
        private readonly int _actionId = 3;
        private readonly string _fileName = "A.txt";

        [TestInitialize]
        public void Setup_OkState()
        {
            _projectValidatorMock = new Mock<IProjectValidator>();

            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsActionAsync(_tagId, _actionId, default)).Returns(Task.FromResult(true));

            _actionValidatorMock = new Mock<IActionValidator>();

            _commandWithoutOverwrite = new UploadActionAttachmentCommand(_tagId, _actionId, _fileName, false, new MemoryStream());

            _dut = new UploadActionAttachmentCommandValidator(_projectValidatorMock.Object, _tagValidatorMock.Object, _actionValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_commandWithoutOverwrite); 

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(_commandWithoutOverwrite.TagId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(_commandWithoutOverwrite.TagId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenFilenameExistsAndNotOverwrite()
        {
            _actionValidatorMock.Setup(r => r.AttachmentWithFilenameExistsAsync(_commandWithoutOverwrite.ActionId, _commandWithoutOverwrite.FileName, default))
                .Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Action already has an attachment with filename {_commandWithoutOverwrite.FileName}"!));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenFilenameExistsAndOverwrite()
        {
            var commandWithOverwrite = new UploadActionAttachmentCommand(_tagId, _actionId, _fileName, true, new MemoryStream());
            _tagValidatorMock.Setup(r => r.AttachmentWithFilenameExistsAsync(commandWithOverwrite.TagId, commandWithOverwrite.FileName, default))
                .Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(commandWithOverwrite);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenActionIsClosed()
        {
            _actionValidatorMock.Setup(r => r.IsClosedAsync(_actionId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Action is closed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenActionNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsActionAsync(_tagId, _actionId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag and/or action doesn't exist!"));
        }
    }
}
