using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.UnvoidRequirementDefinition
{
    [TestClass]
    public class UnvoidRequirementDefinitionCommandValidatorTests
    {
        private Mock<IRequirementDefinitionValidator> _reqDefinitionValidatorMock;
        private Mock<IRequirementTypeValidator> _reqTypeValidatorMock;
        private UnvoidRequirementDefinitionCommand _command;
        private UnvoidRequirementDefinitionCommandValidator _dut;

        private readonly int _requirementTypeId = 1;
        private readonly int _requirementDefinitionId = 2;

        [TestInitialize]
        public void Setup_OkState()
        {
            _reqTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _reqTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            _reqDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _reqDefinitionValidatorMock.Setup(r => r.ExistsAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));
            _reqDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default))
                .Returns(Task.FromResult(true));

            _command = new UnvoidRequirementDefinitionCommand(_requirementTypeId, _requirementDefinitionId, null, Guid.Empty);
            _dut = new UnvoidRequirementDefinitionCommandValidator(_reqTypeValidatorMock.Object, _reqDefinitionValidatorMock.Object);
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
        public void Validate_ShouldFail_WhenRequirementDefinitionIsNotVoided()
        {
            _reqDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is not voided!"));
        }
    }
}
