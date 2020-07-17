using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementDefinition
{
    [TestClass]
    public class CreateRequirementDefinitionCommandValidatorTests
    {
        private CreateRequirementDefinitionCommandValidator _dut;
        private Mock<IRequirementDefinitionValidator> _requirementDefinitionValidatorMock;
        private Mock<IRequirementTypeValidator> _requirementTypeValidatorMock;
        private CreateRequirementDefinitionCommand _command;

        private int ReqTypeId = 1;
        private int SortKey = 10;
        private string Title = "Title";
        private RequirementUsage Usage = RequirementUsage.ForAll;
        private IList<FieldsForCommand> Fields = new List<FieldsForCommand>
        {
            new FieldsForCommand("Label text", FieldType.Attachment, 10)
        };

        [TestInitialize]
        public void Setup_OkState()
        {
            _requirementTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(ReqTypeId, default)).Returns(Task.FromResult(true));

            _requirementDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();

            _command = new CreateRequirementDefinitionCommand(ReqTypeId, SortKey, Usage, Title, 4, Fields);
            _dut = new CreateRequirementDefinitionCommandValidator(_requirementDefinitionValidatorMock.Object, _requirementTypeValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionWithSameTitleAlreadyExistsOnRequirementType()
        {
            var fieldTypes = Fields.Select(f => f.FieldType).ToList();
            _requirementDefinitionValidatorMock.Setup(r => r.IsNotUniqueTitleOnRequirementTypeAsync(ReqTypeId, Title, fieldTypes, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("A requirement definition with this title already exists on the requirement type"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeIsVoided()
        {
            _requirementTypeValidatorMock.Setup(r => r.IsVoidedAsync(ReqTypeId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeDoesNotExist()
        {
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(ReqTypeId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type doesn't exists!"));
        }
    }
}
