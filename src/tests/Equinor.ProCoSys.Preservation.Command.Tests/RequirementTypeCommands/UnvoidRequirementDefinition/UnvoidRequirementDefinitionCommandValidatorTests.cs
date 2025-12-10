using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.UnvoidRequirementDefinition
{
    [TestClass]
    public class UnvoidRequirementDefinitionCommandValidatorTests
    {
        private Mock<IRequirementDefinitionValidator> _reqDefinitionValidatorMock;
        private Mock<IRequirementTypeValidator> _reqTypeValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UnvoidRequirementDefinitionCommand _command;
        private UnvoidRequirementDefinitionCommandValidator _dut;

        private readonly int _requirementTypeId = 1;
        private readonly int _requirementDefinitionId = 2;
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _reqTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _reqTypeValidatorMock
                .Setup(r => r.RequirementDefinitionExistsAsync(_requirementTypeId, _requirementDefinitionId, default))
                .Returns(Task.FromResult(true));

            _reqDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _reqDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default))
                .Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion)).Returns(true);

            _command = new UnvoidRequirementDefinitionCommand(_requirementTypeId, _requirementDefinitionId, _rowVersion);
            _dut = new UnvoidRequirementDefinitionCommandValidator(_reqTypeValidatorMock.Object,
                _reqDefinitionValidatorMock.Object, _rowVersionValidatorMock.Object);
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
            _reqTypeValidatorMock
                .Setup(r => r.RequirementDefinitionExistsAsync(_requirementTypeId, _requirementDefinitionId, default))
                .Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type and/or requirement definition doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementDefinitionIsNotVoided()
        {
            _reqDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is not voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string InvalidRowVersion = "String";

            var command = new UnvoidRequirementDefinitionCommand(_requirementTypeId, _requirementDefinitionId, InvalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(InvalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}
