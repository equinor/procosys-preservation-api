using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.DeleteRequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.DeleteRequirementDefinition
{
    [TestClass]
    public class DeleteRequirementDefinitionCommandValidatorTests
    {
        private DeleteRequirementDefinitionCommandValidator _dut;
        private Mock<IRequirementTypeValidator> _requirementTypeValidatorMock;
        private DeleteRequirementDefinitionCommand _command;

        private int _requirementTypeId = 1;
        private int _requirementDefinitionId = 2;
        private Mock<IRequirementDefinitionValidator> _requirementDefinitionValidatorMock;

        [TestInitialize]
        public void Setup_OkState()
        {
            _requirementTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));
            _requirementDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _requirementDefinitionValidatorMock.Setup(r => r.ExistsAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));
            _requirementDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));
            _command = new DeleteRequirementDefinitionCommand(_requirementTypeId, _requirementDefinitionId, null);

            _dut = new DeleteRequirementDefinitionCommandValidator(_requirementTypeValidatorMock.Object, _requirementDefinitionValidatorMock.Object);
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
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionNotVoided()
        {
            _requirementDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is not voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionExistsOnTagRequirement()
        {
            _requirementDefinitionValidatorMock.Setup(r => r.TagRequirementsExistAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag requirement with this requirement definition exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionExistsOnTagFunctionRequirement()
        {
            _requirementDefinitionValidatorMock.Setup(r => r.TagFunctionRequirementsExistAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag function requirement with this requirement definition exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionHasFields()
        {
            _requirementDefinitionValidatorMock.Setup(r => r.FieldsExistAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition has fields!"));
        }
    }
}
