using System.IO;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagAttachmentCommands.Upload;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagAttachmentCommands.Upload
{
    [TestClass]
    public class UploadTagAttachmentCommandValidatorTests
    {
        private UploadTagAttachmentCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private UploadTagAttachmentCommand _commandWithoutOverwrite;

        private const int TagId = 2;
        private const string FileName = "A.txt";

        [TestInitialize]
        public void Setup_OkState()
        {
            _projectValidatorMock = new Mock<IProjectValidator>();
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId, default)).Returns(Task.FromResult(false));

            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId, default)).Returns(Task.FromResult(true));

            _commandWithoutOverwrite = new UploadTagAttachmentCommand(TagId, FileName, false, new MemoryStream());

            _dut = new UploadTagAttachmentCommandValidator(_projectValidatorMock.Object, _tagValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_commandWithoutOverwrite); 

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(_commandWithoutOverwrite.TagId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_commandWithoutOverwrite.TagId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(_commandWithoutOverwrite.TagId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFilenameExistsAndNotOverwrite()
        {
            _tagValidatorMock.Setup(r => r.AttachmentWithFilenameExistsAsync(_commandWithoutOverwrite.TagId, _commandWithoutOverwrite.FileName, default))
                .Returns(Task.FromResult(true));

            var result = _dut.Validate(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Tag already have an attachment with filename {_commandWithoutOverwrite.FileName}"!));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenFilenameExistsAndOverwrite()
        {
            var commandWithOverwrite = new UploadTagAttachmentCommand(TagId, FileName, true, new MemoryStream());
            _tagValidatorMock.Setup(r => r.AttachmentWithFilenameExistsAsync(commandWithOverwrite.TagId, commandWithOverwrite.FileName, default))
                .Returns(Task.FromResult(true));

            var result = _dut.Validate(commandWithOverwrite);

            Assert.IsTrue(result.IsValid);
        }
    }
}
