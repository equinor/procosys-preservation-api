using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementType;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.VoidRequirementType
{
    [TestClass]
    public class VoidRequirementTypeCommandValidatorTests
    {
        private Mock<IRequirementTypeValidator> _reqTypeValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;

        private VoidRequirementTypeCommand _command;
        private VoidRequirementTypeCommandValidator _dut;
        private readonly int _requirementTypeId = 2;
        private string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _reqTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _reqTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion, default)).Returns(Task.FromResult(true));

            _command = new VoidRequirementTypeCommand(_requirementTypeId, _rowVersion);
            _dut = new VoidRequirementTypeCommandValidator(_reqTypeValidatorMock.Object, _rowVersionValidatorMock.Object);
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
        public void Validate_ShouldFail_WhenRequirementTypeIsVoided()
        {
            _reqTypeValidatorMock.Setup(r => r.IsVoidedAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type is already voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new VoidRequirementTypeCommand(_requirementTypeId, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid RowVersion!"));
        }
    }
}
