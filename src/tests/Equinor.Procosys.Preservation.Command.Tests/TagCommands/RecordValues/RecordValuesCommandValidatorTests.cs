using System.Collections.Generic;
using Equinor.Procosys.Preservation.Command.TagCommands.RecordValues;
using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.RecordValues
{
    [TestClass]
    public class RecordValuesCommandValidatorTests
    {
        private const string Comment = "Comment";
        private const int TagId = 1;
        private const int FieldId = 11;
        private const double Number = 1282.91;
        private const int ReqId = 21;

        private RecordValuesCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IFieldValidator> _fieldValidatorMock;
        private RecordValuesCommand _recordValuesCommandWithCommentOnly;
        private RecordValuesCommand _recordValuesCommandWithCheckedCheckBox;
        private RecordValuesCommand _recordValuesCommandWithUncheckedCheckBox;
        private RecordValuesCommand _recordValuesCommandWithNaNumber;
        private RecordValuesCommand _recordValuesCommandWithNullNumber;
        private RecordValuesCommand _recordValuesCommandWithNormalNumber;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(v => v.Exists(TagId)).Returns(true);
            _tagValidatorMock.Setup(v => v.RequirementIsReadyForRecording(ReqId)).Returns(true);

            _fieldValidatorMock = new Mock<IFieldValidator>();
            _fieldValidatorMock.Setup(v => v.Exists(FieldId)).Returns(true);

            _recordValuesCommandWithCommentOnly = new RecordValuesCommand(
                TagId,
                ReqId, 
                null, 
                Comment);

            _recordValuesCommandWithCheckedCheckBox = new RecordValuesCommand(
                TagId,
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(FieldId, "true")
                }, 
                Comment);
            
            _recordValuesCommandWithUncheckedCheckBox = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(FieldId, "false")
                }, 
                Comment);
            
            _recordValuesCommandWithNaNumber = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(FieldId, "n/a")
                }, 
                Comment);
            
            _recordValuesCommandWithNullNumber = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(FieldId, null)
                }, 
                Comment);

            _recordValuesCommandWithNormalNumber = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new List<FieldValue>
                {
                    new FieldValue(FieldId, Number.ToString("F2"))
                }, 
                Comment);

            _dut = new RecordValuesCommandValidator(_tagValidatorMock.Object, _fieldValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenCommentOnly()
        {
            var result = _dut.Validate(_recordValuesCommandWithCommentOnly);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotExists()
        {
            _tagValidatorMock.Setup(v => v.Exists(TagId)).Returns(false);
            
            var result = _dut.Validate(_recordValuesCommandWithCommentOnly);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoided(TagId)).Returns(true);
            
            var result = _dut.Validate(_recordValuesCommandWithCommentOnly);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _tagValidatorMock.Setup(r => r.ProjectIsClosed(TagId)).Returns(true);
            
            var result = _dut.Validate(_recordValuesCommandWithCommentOnly);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementIsNotReadyForRecording()
        {
            _tagValidatorMock.Setup(r => r.RequirementIsReadyForRecording(ReqId)).Returns(false);
            
            var result = _dut.Validate(_recordValuesCommandWithCommentOnly);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("The requirement for the field is not ready for recording!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyFieldNotExists()
        {
            _fieldValidatorMock.Setup(r => r.Exists(FieldId)).Returns(false);
            
            var result = _dut.Validate(_recordValuesCommandWithNormalNumber);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyFieldIsVoided()
        {
            _fieldValidatorMock.Setup(r => r.IsVoided(FieldId)).Returns(true);
            
            var result = _dut.Validate(_recordValuesCommandWithNormalNumber);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field is voided!"));
        }
    }
}
