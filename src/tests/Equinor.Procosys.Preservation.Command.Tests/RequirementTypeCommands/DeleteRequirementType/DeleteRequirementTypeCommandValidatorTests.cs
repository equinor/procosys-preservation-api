using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.DeleteRequirementType;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.DeleteRequirementType
{
    [TestClass]
    public class DeleteRequirementTypeCommandValidatorTests
    {
        private DeleteRequirementTypeCommandValidator _dut;
        private Mock<IRequirementTypeValidator> _requirementTypeValidatorMock;
        private DeleteRequirementTypeCommand _command;

        private int _id = 1;

        [TestInitialize]
        public void Setup_OkState()
        {
            _requirementTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(true));
            _requirementTypeValidatorMock.Setup(r => r.IsVoidedAsync(_id, default)).Returns(Task.FromResult(true));
            _command = new DeleteRequirementTypeCommand(_id, null);

            _dut = new DeleteRequirementTypeCommandValidator(_requirementTypeValidatorMock.Object);
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
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeNotVoided()
        {
            _requirementTypeValidatorMock.Setup(r => r.IsVoidedAsync(_id, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type is not voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeHasDefinitions()
        {
            _requirementTypeValidatorMock.Setup(r => r.AnyRequirementDefinitionExistsAsync(_id, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type has requirement definitions!"));
        }
    }
}
