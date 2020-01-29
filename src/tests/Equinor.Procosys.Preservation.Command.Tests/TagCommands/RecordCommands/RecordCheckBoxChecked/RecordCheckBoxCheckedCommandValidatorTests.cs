using Equinor.Procosys.Preservation.Command.TagCommands.RecordCommands.RecordCheckBoxChecked;
using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.RecordCommands.RecordCheckBoxChecked
{
    [TestClass]
    public class RecordCheckBoxCheckedCommandValidatorTests
    {
        private RecordCheckBoxCheckedCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IFieldValidator> _fieldValidatorMock;
        private RecordCheckBoxCheckedCommand _command;

        private int _tagId = 1;
        private int _fieldId = 2;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.Exists(_tagId)).Returns(true);
            _tagValidatorMock.Setup(r => r.RequirementIsReadyForRecording(_tagId, _fieldId)).Returns(true);
            _fieldValidatorMock = new Mock<IFieldValidator>();
            _fieldValidatorMock.Setup(r => r.Exists(_fieldId)).Returns(true);
            _fieldValidatorMock.Setup(r => r.VerifyFieldType(_fieldId, FieldType.CheckBox)).Returns(true);
            
            _command = new RecordCheckBoxCheckedCommand(_tagId, _fieldId, true);

            _dut = new RecordCheckBoxCheckedCommandValidator(
                _tagValidatorMock.Object, 
                _fieldValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenFieldNotCheckBox()
        {
            _fieldValidatorMock.Setup(r => r.VerifyFieldType(_fieldId, FieldType.CheckBox)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Field is not of type {FieldType.CheckBox}!"));
        }
        // RecordCommandValidatorTests covers other unit test for RecordCheckBoxCheckedCommandValidatorTests
    }
}
