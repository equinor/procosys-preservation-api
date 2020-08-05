using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition;
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
        private Mock<IRequirementTypeValidator> _requirementTypeValidatorMock;
        private CreateRequirementDefinitionCommand _command;

        private readonly int _reqTypeId = 1;
        private readonly int _sortKey = 10;
        private readonly string _title = "Title";
        private RequirementUsage Usage = RequirementUsage.ForAll;
        private readonly IList<FieldsForCommand> _fields = new List<FieldsForCommand>
        {
            new FieldsForCommand("Label text", FieldType.Attachment, 10)
        };

        [TestInitialize]
        public void Setup_OkState()
        {
            _requirementTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(_reqTypeId, default)).Returns(Task.FromResult(true));

            _command = new CreateRequirementDefinitionCommand(_reqTypeId, _sortKey, Usage, _title, 4, _fields);
            _dut = new CreateRequirementDefinitionCommandValidator(_requirementTypeValidatorMock.Object);
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
            var fieldTypes = _fields.Select(f => f.FieldType).ToList();
            _requirementTypeValidatorMock.Setup(r => r.AnyRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId, _title, fieldTypes, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("A requirement definition with this title already exists on the requirement type"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeIsVoided()
        {
            _requirementTypeValidatorMock.Setup(r => r.IsVoidedAsync(_reqTypeId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeDoesNotExist()
        {
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(_reqTypeId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type doesn't exists!"));
        }

        // todo test adding of fields
    }
}
