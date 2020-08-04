using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementType;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.UpdateRequirementType
{
    [TestClass]
    public class UpdateRequirementTypeCommandValidatorTests
    {
        private Mock<IRequirementTypeValidator> _reqTypeValidatorMock;

        private UpdateRequirementTypeCommand _command;
        private UpdateRequirementTypeCommandValidator _dut;
        private readonly int _requirementTypeId = 2;
        private readonly int _sortKey = 1;
        private readonly string _title = "Title test";
        private readonly string _code = "Code test";
        private RequirementTypeIcon _icon = RequirementTypeIcon.Other;

        [TestInitialize]
        public void Setup_OkState()
        {
            _reqTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _reqTypeValidatorMock.Setup(r => r.ExistsAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            _command = new UpdateRequirementTypeCommand(_requirementTypeId, null, _sortKey, _title, _code, _icon);
            _dut = new UpdateRequirementTypeCommandValidator(_reqTypeValidatorMock.Object);
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
        public void Validate_ShouldFail_WhenRequirementTypeIsVoided()
        {
            _reqTypeValidatorMock.Setup(r => r.IsVoidedAsync(_requirementTypeId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeTitleIsNotUnique()
        {
            _reqTypeValidatorMock.Setup(r => r.ExistsWithSameTitleInAnotherTypeAsync(_requirementTypeId, _title, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Another requirement type with this title already exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeCodeIsNotUnique()
        {
            _reqTypeValidatorMock.Setup(r => r.ExistsWithSameCodeInAnotherTypeAsync(_requirementTypeId, _code, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Another requirement type with this code already exists!"));
        }
    }
}
