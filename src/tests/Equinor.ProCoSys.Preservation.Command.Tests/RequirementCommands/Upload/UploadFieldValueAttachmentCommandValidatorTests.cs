using System.IO;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementCommands.Upload;
using Equinor.ProCoSys.Preservation.Command.Validators.FieldValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementCommands.Upload
{
    [TestClass]
    public class UploadFieldValueAttachmentCommandValidatorTests
    {
        private const int TagId = 1;
        private const int AttachmentFieldId = 11;
        private const int ReqId = 21;

        private UploadFieldValueAttachmentCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IFieldValidator> _fieldValidatorMock;
        private UploadFieldValueAttachmentCommand _command;

        [TestInitialize]
        public void Setup_OkState()
        {
            _projectValidatorMock = new Mock<IProjectValidator>();
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(v => v.ExistsRequirementAsync(TagId, ReqId, default))
                .Returns(Task.FromResult(true));
            _tagValidatorMock
                .Setup(v => v.ExistsFieldForRequirementAsync(TagId, ReqId, AttachmentFieldId, default))
                .Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(v => v.HasRequirementWithActivePeriodAsync(TagId, ReqId, default))
                .Returns(Task.FromResult(true));
            _fieldValidatorMock = new Mock<IFieldValidator>();
            _fieldValidatorMock.Setup(r => r.IsValidForAttachmentAsync(AttachmentFieldId, default))
                .Returns(Task.FromResult(true));

            _command = new UploadFieldValueAttachmentCommand(
                TagId,
                ReqId,
                AttachmentFieldId,
                "F",
                new MemoryStream());

            _dut = new UploadFieldValueAttachmentCommandValidator(_projectValidatorMock.Object, _tagValidatorMock.Object, _fieldValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagOrReqNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsRequirementAsync(TagId, ReqId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag and/or requirement doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenFieldNotExistsForRequirement()
        {
            _tagValidatorMock
                .Setup(v => v.ExistsFieldForRequirementAsync(TagId, ReqId, AttachmentFieldId, default))
                .Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field doesn't exist in requirement!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(TagId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementDontHaveActivePeriod()
        {
            _tagValidatorMock.Setup(v => v.HasRequirementWithActivePeriodAsync(TagId, ReqId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't have this requirement with active period!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenFieldNotForAttachment()
        {
            _fieldValidatorMock.Setup(r => r.IsValidForAttachmentAsync(AttachmentFieldId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field values can not be recorded for field type!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(v => v.HasRequirementWithActivePeriodAsync(TagId, ReqId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }
    }
}
