using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementCommands.DeleteAttachment;
using Equinor.Procosys.Preservation.Command.Validators.FieldValidators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementCommands.DeleteAttachment
{
    [TestClass]
    public class DeleteFieldValueAttachmentCommandValidatorTests
    {
        private const int TagId = 1;
        private const int AttachmentFieldId = 11;
        private const int RequirementId = 21;

        private DeleteFieldValueAttachmentCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IFieldValidator> _fieldValidatorMock;
        private DeleteFieldValueAttachmentCommand _command;

        [TestInitialize]
        public void Setup_OkState()
        {
            _projectValidatorMock = new Mock<IProjectValidator>();
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(v => v.ExistsRequirementAsync(TagId, RequirementId, default))
                .Returns(Task.FromResult(true));
            _tagValidatorMock
                .Setup(v => v.ExistsFieldForRequirementAsync(TagId, RequirementId, AttachmentFieldId, default))
                .Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(v => v.HasRequirementWithActivePeriodAsync(TagId, RequirementId, default))
                .Returns(Task.FromResult(true));
            _fieldValidatorMock = new Mock<IFieldValidator>();
            _fieldValidatorMock.Setup(r => r.IsValidForAttachmentAsync(AttachmentFieldId, default))
                .Returns(Task.FromResult(true));

            _command = new DeleteFieldValueAttachmentCommand(
                TagId, 
                RequirementId, 
                AttachmentFieldId);

            _dut = new DeleteFieldValueAttachmentCommandValidator(
                _projectValidatorMock.Object,
                _tagValidatorMock.Object,
                _fieldValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagOrReqNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsRequirementAsync(TagId, RequirementId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag and/or requirement doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldNotExistsForRequirement()
        {
            _tagValidatorMock
                .Setup(v => v.ExistsFieldForRequirementAsync(TagId, RequirementId, AttachmentFieldId, default))
                .Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field doesn't exist for requirement!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(TagId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDontHaveActivePeriod()
        {
            _tagValidatorMock.Setup(v => v.HasRequirementWithActivePeriodAsync(TagId, RequirementId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't have this requirement with active period!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldNotForAttachment()
        {
            _fieldValidatorMock.Setup(r => r.IsValidForAttachmentAsync(AttachmentFieldId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Attachment can not be uploaded for field!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(v => v.HasRequirementWithActivePeriodAsync(TagId, RequirementId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }
    }
}
