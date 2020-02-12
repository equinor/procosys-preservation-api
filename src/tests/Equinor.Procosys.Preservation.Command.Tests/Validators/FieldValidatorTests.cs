using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class FieldValidatorTests
    {
        private const int InfoFieldIdNonVoided = 1;
        private const int InfoFieldIdVoided = 2;
        private const int CheckBoxFieldId = 3;
        private const int NumberFieldId = 4;
        private FieldValidator _dut;

        [TestInitialize]
        public void Setup()
        {
            var rtRepoMock = new Mock<IRequirementTypeRepository>();

            var nonVoidedInfoField = new Field("", "", FieldType.Info, 0);
            var voidedInfoField = new Field("", "", FieldType.Info, 0);
            voidedInfoField.Void();
            var checkBoxField = new Field("", "", FieldType.CheckBox, 0);
            var numberField = new Field("", "", FieldType.Number, 0, "mm", true);

            rtRepoMock.Setup(r => r.GetFieldByIdAsync(InfoFieldIdNonVoided)).Returns(Task.FromResult(nonVoidedInfoField));
            rtRepoMock.Setup(r => r.GetFieldByIdAsync(InfoFieldIdVoided)).Returns(Task.FromResult(voidedInfoField));
            rtRepoMock.Setup(r => r.GetFieldByIdAsync(CheckBoxFieldId)).Returns(Task.FromResult(checkBoxField));
            rtRepoMock.Setup(r => r.GetFieldByIdAsync(NumberFieldId)).Returns(Task.FromResult(numberField));

            _dut = new FieldValidator(rtRepoMock.Object);
        }

        [TestMethod]
        public void Exists_KnownId_ReturnsTrue()
        {
            var result = _dut.Exists(InfoFieldIdNonVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Exists_UnknownId_ReturnsFalse()
        {
            var result = _dut.Exists(126234);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsVoided_KnownVoided_ReturnsTrue()
        {
            var result = _dut.IsVoided(InfoFieldIdVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsVoided_KnownNotVoided_ReturnsFalse()
        {
            var result = _dut.IsVoided(InfoFieldIdNonVoided);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsVoided_UnknownId_ReturnsFalse()
        {
            var result = _dut.IsVoided(126234);
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void IsValidValue_ReturnsTrue_OnNumberField_ForNA()
        {
            var result = _dut.IsValidValue(NumberFieldId, "NA");
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void IsValidValue_ReturnsTrue_OnNumberField_ForNAWithSlash()
        {
            var result = _dut.IsValidValue(NumberFieldId, "n/a");
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void IsValidValue_OnNumberField_IsNotCaseSensitive()
        {
            var result = _dut.IsValidValue(NumberFieldId, "nA");
            Assert.IsTrue(result);
            result = _dut.IsValidValue(NumberFieldId, "n/A");
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void IsValidValue_ReturnsTrue_OnNumberField_ForNumber()
        {
            var result = _dut.IsValidValue(NumberFieldId, "12");
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void IsValidValue_ReturnsFalse_OnNumberField_ForText()
        {
            var result = _dut.IsValidValue(NumberFieldId, "abc");
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void IsValidValue_ReturnsTrue_OnCheckBoxField_ForTrue()
        {
            var result = _dut.IsValidValue(CheckBoxFieldId, "True");
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void IsValidValue_OnCheckBoxField_IsNotCaseSensitive()
        {
            var result = _dut.IsValidValue(CheckBoxFieldId, "TruE");
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void IsValidValue_ReturnsTrue_OnCheckBoxField_ForFalse()
        {
            var result = _dut.IsValidValue(CheckBoxFieldId, "False");
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void IsValidValue_ReturnsFalse_OnCheckBoxField_ForText()
        {
            var result = _dut.IsValidValue(CheckBoxFieldId, "abc");
            Assert.IsFalse(result);
        }

    }
}
