using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.UpdateRequirementDefinition
{
    [TestClass]
    public class UpdateRequirementDefinitionCommandValidatorTests
    {
        private Mock<IRequirementTypeValidator> _reqTypeValidatorMock;
        private Mock<IRequirementDefinitionValidator> _reqDefinitionValidatorMock;

        private UpdateRequirementDefinitionCommand _command;
        private UpdateRequirementDefinitionCommandValidator _dut;
        private readonly int _requirementTypeId = 2;
        private readonly int _requirementDefinitionId = 3;
        private readonly int _sortKey = 10;
        private readonly string _title = "Title test";
        private RequirementUsage _usage = RequirementUsage.ForAll;
        private int _defaultIntervalWeeks = 4;
        private string _rowVersion = "AAAAAAAAABA=";
        private IList<UpdateFieldsForCommand> _updatedFields = new List<UpdateFieldsForCommand>();
        private IList<FieldsForCommand> _newFields = new List<FieldsForCommand>();


        [TestInitialize]
        public void Setup_OkState()
        {
            _reqTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _reqTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            _reqDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _reqDefinitionValidatorMock.Setup(rd => rd.ExistsAsync(_requirementDefinitionId, default))
                .Returns(Task.FromResult(true));

            _command = new UpdateRequirementDefinitionCommand(
                _requirementTypeId, 
                _requirementDefinitionId, 
                _sortKey, 
                _usage, 
                _title, 
                _defaultIntervalWeeks, 
                _rowVersion, 
                _updatedFields, 
                _newFields);
            _dut = new UpdateRequirementDefinitionCommandValidator(_reqTypeValidatorMock.Object, _reqDefinitionValidatorMock.Object);
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
        public void Validate_ShouldFail_WhenRequirementDefinitionNotExists()
        {
            _reqDefinitionValidatorMock.Setup(r => r.ExistsAsync(_requirementDefinitionId, default))
                .Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeIsVoided()
        {
            _reqTypeValidatorMock.Setup(r => r.IsVoidedAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionIsVoided()
        {
            _reqDefinitionValidatorMock.Setup(r => r.IsVoidedAsync(_requirementDefinitionId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementDefinitionTitleIsNotUniqueOnType()
        {
            var fieldTypes1 = _updatedFields.Select(f => f.FieldType).ToList();
            var fieldTypes2 = _newFields.Select(f => f.FieldType).ToList();
            var fieldTypesConcatenated = fieldTypes1.Concat(fieldTypes2).ToList();

            _reqDefinitionValidatorMock
                .Setup(r => r.IsNotUniqueTitleOnRequirementTypeAsync(
                    _requirementTypeId, 
                    _title, 
                    fieldTypesConcatenated,
                    default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("A requirement definition with this title already exists on the requirement type"));
        }
    }
}
