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
        private static readonly string NumberAsString = Number.ToString("F2");
        private const int ReqId = 21;

        private RecordValuesCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IFieldValidator> _fieldValidatorMock;
        private RecordValuesCommand _recordValuesCommandWithCommentOnly;
        private RecordValuesCommand _recordValuesCommandWithNormalNumber;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(v => v.Exists(TagId)).Returns(true);
            _tagValidatorMock.Setup(v => v.HaveRequirementReadyForRecording(TagId, ReqId)).Returns(true);

            _fieldValidatorMock = new Mock<IFieldValidator>();
            _fieldValidatorMock.Setup(v => v.Exists(FieldId)).Returns(true);
            _fieldValidatorMock.Setup(r => r.IsValidValue(FieldId, It.IsAny<string>())).Returns(true);

            _recordValuesCommandWithCommentOnly = new RecordValuesCommand(
                TagId,
                ReqId, 
                null, 
                Comment);

            _recordValuesCommandWithNormalNumber = new RecordValuesCommand(
                TagId, 
                ReqId, 
                new Dictionary<int, string> {{FieldId, NumberAsString}},
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
            _tagValidatorMock.Setup(v => v.HaveRequirementReadyForRecording(TagId, ReqId)).Returns(false);
            
            var result = _dut.Validate(_recordValuesCommandWithCommentOnly);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't have this requirement ready for recording!"));
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
        public void Validate_ShouldFail_WhenAnyFieldNotValid()
        {
            _fieldValidatorMock.Setup(r => r.IsValidValue(FieldId, NumberAsString)).Returns(false);
            
            var result = _dut.Validate(_recordValuesCommandWithNormalNumber);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field value is not valid!"));
        }
 
        [TestMethod]
        public void Validate_ShouldFailWith2Errors_WhenErrorsIn2Rules()
        {
            _tagValidatorMock.Setup(r => r.ProjectIsClosed(TagId)).Returns(true);
            _fieldValidatorMock.Setup(r => r.IsValidValue(FieldId, NumberAsString)).Returns(false);

            var result = _dut.Validate(_recordValuesCommandWithNormalNumber);
            
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
        }
    }
}
