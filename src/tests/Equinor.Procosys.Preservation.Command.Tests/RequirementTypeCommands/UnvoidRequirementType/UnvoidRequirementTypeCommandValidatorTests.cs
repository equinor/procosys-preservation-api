using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementType;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.UnvoidRequirementType
{
    [TestClass]
    public class UnvoidRequirementTypeCommandValidatorTests
    {
        private Mock<IRequirementTypeValidator> _reqTypeValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;

        private UnvoidRequirementTypeCommand _command;
        private UnvoidRequirementTypeCommandValidator _dut;
        private readonly int _requirementTypeId = 2;
        private string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _reqTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _reqTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));
            _reqTypeValidatorMock.Setup(r => r.IsVoidedAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion, default)).Returns(Task.FromResult(true));

            _command = new UnvoidRequirementTypeCommand(_requirementTypeId, _rowVersion);
            _dut = new UnvoidRequirementTypeCommandValidator(_reqTypeValidatorMock.Object, _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeNotExists()
        {
            _reqTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeIsNotVoided()
        {
            _reqTypeValidatorMock.Setup(r => r.IsVoidedAsync(_requirementTypeId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type is not voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new UnvoidRequirementTypeCommand(_requirementTypeId, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid RowVersion!"));
        }
    }
}
