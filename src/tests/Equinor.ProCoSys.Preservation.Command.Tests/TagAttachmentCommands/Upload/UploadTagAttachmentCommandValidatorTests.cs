using System.IO;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagAttachmentCommands.Upload;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagAttachmentCommands.Upload
{
    [TestClass]
    public class UploadTagAttachmentCommandValidatorTests
    {
        private UploadTagAttachmentCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private UploadTagAttachmentCommand _commandWithoutOverwrite;

        private readonly string _fileName = "A.txt";

        [TestInitialize]
        public void Setup_OkState()
        {
            _commandWithoutOverwrite = new UploadTagAttachmentCommand(2, _fileName, false, new MemoryStream());

            _projectValidatorMock = new Mock<IProjectValidator>();

            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsAsync(_commandWithoutOverwrite.TagId, default)).Returns(Task.FromResult(true));

            _dut = new UploadTagAttachmentCommandValidator(_projectValidatorMock.Object, _tagValidatorMock.Object);
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
        public async Task Validate_ShouldFail_WhenTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_commandWithoutOverwrite.TagId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exist!"));
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
            _tagValidatorMock.Setup(r => r.AttachmentWithFilenameExistsAsync(_commandWithoutOverwrite.TagId, _commandWithoutOverwrite.FileName, default))
                .Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Tag already has an attachment with filename {_commandWithoutOverwrite.FileName}"!));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenFilenameExistsAndOverwrite()
        {
            var commandWithOverwrite = new UploadTagAttachmentCommand(2, _fileName, true, new MemoryStream());
            _tagValidatorMock.Setup(r => r.AttachmentWithFilenameExistsAsync(commandWithOverwrite.TagId, commandWithOverwrite.FileName, default))
                .Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(commandWithOverwrite);

            Assert.IsTrue(result.IsValid);
        }
    }
}
