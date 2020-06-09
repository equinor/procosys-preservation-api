using System.IO;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionAttachmentCommands.Upload;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Command.Validators.ActionValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionAttachmentCommands.Upload
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
            _tagValidatorMock.Setup(r => r.ExistsAsync(_tagId, default)).Returns(Task.FromResult(true));

            _actionValidatorMock = new Mock<IActionValidator>();
            _actionValidatorMock.Setup(r => r.ExistsAsync(_actionId, default)).Returns(Task.FromResult(true));

            _commandWithoutOverwrite = new UploadActionAttachmentCommand(_tagId, _actionId, _fileName, false, new MemoryStream(), Guid.Empty);

            _dut = new UploadActionAttachmentCommandValidator(_projectValidatorMock.Object, _tagValidatorMock.Object, _actionValidatorMock.Object);
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
            _actionValidatorMock.Setup(r => r.AttachmentWithFilenameExistsAsync(_commandWithoutOverwrite.ActionId, _commandWithoutOverwrite.FileName, default))
                .Returns(Task.FromResult(true));

            var result = _dut.Validate(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Action already has an attachment with filename {_commandWithoutOverwrite.FileName}"!));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenFilenameExistsAndOverwrite()
        {
            var commandWithOverwrite = new UploadActionAttachmentCommand(_tagId, _actionId, _fileName, true, new MemoryStream(), Guid.Empty);
            _tagValidatorMock.Setup(r => r.AttachmentWithFilenameExistsAsync(commandWithOverwrite.TagId, commandWithOverwrite.FileName, default))
                .Returns(Task.FromResult(true));

            var result = _dut.Validate(commandWithOverwrite);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenActionIsClosed()
        {
            _actionValidatorMock.Setup(r => r.IsClosedAsync(_actionId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Action is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenActionNotExists()
        {
            _actionValidatorMock.Setup(r => r.ExistsAsync(_actionId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_commandWithoutOverwrite);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Action doesn't exist!"));
        }
    }
}
