using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementDefinition
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
        private RequirementUsage _usage = RequirementUsage.ForAll;
        private readonly IList<FieldsForCommand> _fields = new List<FieldsForCommand>
        {
            new FieldsForCommand("Label text", FieldType.Attachment, 10)
        };

        [TestInitialize]
        public void Setup_OkState()
        {
            _requirementTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(_reqTypeId, default)).Returns(Task.FromResult(true));

            _command = new CreateRequirementDefinitionCommand(_reqTypeId, _sortKey, _usage, _title, 4, _fields);
            _dut = new CreateRequirementDefinitionCommandValidator(_requirementTypeValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementDefinitionWithSameTitleAlreadyExistsOnRequirementType()
        {
            var fieldTypes = _fields.Select(f => f.FieldType).ToList();
            _requirementTypeValidatorMock.Setup(r => r.AnyRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId, _title, fieldTypes, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("A requirement definition with this title already exists on the requirement type"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementTypeIsVoided()
        {
            _requirementTypeValidatorMock.Setup(r => r.IsVoidedAsync(_reqTypeId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementTypeDoesNotExist()
        {
            _requirementTypeValidatorMock.Setup(r => r.ExistsAsync(_reqTypeId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type doesn't exist!"));
        }
    }
}
