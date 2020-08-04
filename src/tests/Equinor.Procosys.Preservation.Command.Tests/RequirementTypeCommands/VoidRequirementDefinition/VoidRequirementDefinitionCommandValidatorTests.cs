using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.VoidRequirementDefinition
{
    [TestClass]
    public class VoidRequirementDefinitionCommandValidatorTests
    {
        private Mock<IRequirementDefinitionValidator> _reqDefinitionValidatorMock;
        private Mock<IRequirementTypeValidator> _reqTypeValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private VoidRequirementDefinitionCommand _command;
        private VoidRequirementDefinitionCommandValidator _dut;

        private readonly int _requirementTypeId = 1;
        private readonly int _requirementDefinitionId = 2;
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _reqTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _reqTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            _reqDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _reqDefinitionValidatorMock.Setup(r => r.ExistsAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new VoidRequirementDefinitionCommand(_requirementTypeId, _requirementDefinitionId, _rowVersion);
            _dut = new VoidRequirementDefinitionCommandValidator(_reqTypeValidatorMock.Object, _reqDefinitionValidatorMock.Object, _rowVersionValidatorMock.Object);
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
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type does not exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionNotExists()
        {
            _reqDefinitionValidatorMock.Setup(r => r.ExistsAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition does not exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionIsVoided()
        {
            _reqDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is already voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new VoidRequirementDefinitionCommand(_requirementTypeId, _requirementDefinitionId, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid RowVersion!"));
        }
    }
}
