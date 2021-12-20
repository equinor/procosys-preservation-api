using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTag;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.UpdateTag
{
    [TestClass]
    public class UpdateTagCommandValidatorTests
    {
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;

        private UpdateTagCommand _command;
        private UpdateTagCommandValidator _dut;

        private readonly int _tagId = 2;
        private readonly string _remark = "Remark";
        private readonly string _storageArea = "StorageArea";
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _projectValidatorMock = new Mock<IProjectValidator>();

            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsAsync(_tagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToBeEditedAsync(_tagId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new UpdateTagCommand(_tagId, _remark, _storageArea, _rowVersion);
            _dut = new UpdateTagCommandValidator(_projectValidatorMock.Object, _tagValidatorMock.Object, _rowVersionValidatorMock.Object);
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
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(_tagId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
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
        public void Validate_ShouldFail_WhenTagCantBeEdited()
        {
            _tagValidatorMock.Setup(r => r.IsReadyToBeEditedAsync(_tagId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag can't be edited!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(_tagId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new UpdateTagCommand(_tagId, _remark, _storageArea, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }

        [TestMethod]
        public void Validate_ShouldIncludeTagNoInMessage()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(_tagId, default)).Returns(Task.FromResult(true));
            var tagNo = "Tag X";
            _tagValidatorMock.Setup(r => r.GetTagDetailsAsync(_tagId, default)).Returns(Task.FromResult(tagNo));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(tagNo));
        }
    }
}
