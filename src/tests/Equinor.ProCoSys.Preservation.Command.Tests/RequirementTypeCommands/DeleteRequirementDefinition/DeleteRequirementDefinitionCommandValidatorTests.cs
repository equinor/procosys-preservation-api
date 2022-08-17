using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.DeleteRequirementDefinition;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.DeleteRequirementDefinition
{
    [TestClass]
    public class DeleteRequirementDefinitionCommandValidatorTests
    {
        private DeleteRequirementDefinitionCommandValidator _dut;
        private Mock<IRequirementTypeValidator> _requirementTypeValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private DeleteRequirementDefinitionCommand _command;

        private int _requirementTypeId = 1;
        private int _requirementDefinitionId = 2;
        private readonly string _rowVersion = "AAAAAAAAJ00=";
        private Mock<IRequirementDefinitionValidator> _requirementDefinitionValidatorMock;

        [TestInitialize]
        public void Setup_OkState()
        {
            _requirementTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _requirementTypeValidatorMock
                .Setup(r => r.RequirementDefinitionExistsAsync(_requirementTypeId, _requirementDefinitionId, default))
                .Returns(Task.FromResult(true));

            _requirementDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _requirementDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new DeleteRequirementDefinitionCommand(_requirementTypeId, _requirementDefinitionId, _rowVersion);

            _dut = new DeleteRequirementDefinitionCommandValidator(
                _requirementTypeValidatorMock.Object,
                _requirementDefinitionValidatorMock.Object,
                _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementDefinitionDoesNotExists()
        {
            _requirementTypeValidatorMock
                .Setup(r => r.RequirementDefinitionExistsAsync(_requirementTypeId, _requirementDefinitionId, default))
                .Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type and/or requirement definition doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementDefinitionNotVoided()
        {
            _requirementDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is not voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementDefinitionExistsOnTagRequirement()
        {
            _requirementDefinitionValidatorMock.Setup(r => r.TagRequirementsExistAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag requirement with this requirement definition exists!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementDefinitionExistsOnTagFunctionRequirement()
        {
            _requirementDefinitionValidatorMock.Setup(r => r.TagFunctionRequirementsExistAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag function requirement with this requirement definition exists!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementDefinitionHasFields()
        {
            _requirementDefinitionValidatorMock.Setup(r => r.HasAnyFieldsAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition has fields!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new DeleteRequirementDefinitionCommand(_requirementTypeId, _requirementDefinitionId, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
