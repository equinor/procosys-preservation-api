using Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands;
using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.FieldValue;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.RecordCommands
{
    [TestClass]
    public class RecordCommandValidatorTests
    {
        private RecordCommandValidator<TestCommand> _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IFieldValidator> _fieldValidatorMock;
        private Mock<IFieldValueValidator> _fieldValueValidatorMock;
        private TestCommand _command;

        private int _tagId = 1;
        private int _fieldId = 2;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.Exists(_tagId)).Returns(true);
            _fieldValidatorMock = new Mock<IFieldValidator>();
            _fieldValidatorMock.Setup(r => r.Exists(_fieldId)).Returns(true);
            _fieldValueValidatorMock = new Mock<IFieldValueValidator>();
            _fieldValueValidatorMock.Setup(r => r.ExistsInCurrentPeriod(_fieldId)).Returns(false);
            
            _command = new TestCommand(_tagId, _fieldId);

            _dut = new RecordCommandValidator<TestCommand>(
                _tagValidatorMock.Object, 
                _fieldValidatorMock.Object,
                _fieldValueValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.Exists(_tagId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldNotExists()
        {
            _fieldValidatorMock.Setup(r => r.Exists(_fieldId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoided(_tagId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldIsVoided()
        {
            _fieldValidatorMock.Setup(r => r.IsVoided(_fieldId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldValueAlreadyExistsInPeriod()
        {
            _fieldValueValidatorMock.Setup(r => r.ExistsInCurrentPeriod(_fieldId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Field value already exists for field in current period!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _tagValidatorMock.Setup(r => r.ProjectIsClosed(_tagId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith2Errors_WhenErrorsInDifferentRules()
        {
            _tagValidatorMock.Setup(r => r.Exists(_tagId)).Returns(false);
            _fieldValidatorMock.Setup(r => r.Exists(_fieldId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
        }
    }
}
