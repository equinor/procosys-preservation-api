using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagFunctionCommands.UpdateRequirements;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagFunctionCommands.UpdateRequirements
{
    [TestClass]
    public class UpdateRequirementsCommandValidatorTests
    {
        private UpdateRequirementsCommandValidator _dut;
        private Mock<IRequirementDefinitionValidator> _rdValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UpdateRequirementsCommand _command;
        private int _rd1Id = 2;
        private int _rd2Id = 3;
        private string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd1Id, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd2Id, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion, default)).Returns(Task.FromResult(true));

            _command = new UpdateRequirementsCommand("", "",
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rd1Id, 1),
                    new RequirementForCommand(_rd2Id, 1)
                },
                _rowVersion);

            _dut = new UpdateRequirementsCommandValidator(_rdValidatorMock.Object, _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WithoutRequirements()
        {
            var command = new UpdateRequirementsCommand("", "", new List<RequirementForCommand>(), _rowVersion);
            var result = _dut.Validate(command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyRequirementDefinitionNotExists()
        {
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd2Id, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition doesn't exists!"));
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(_rd2Id.ToString()));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementUsageForSupplierNotGiven()
        {
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.UsageCoversForOtherThanSuppliersAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used both for supplier and other than suppliers!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyRequirementDefinitionIsVoided()
        {
            _rdValidatorMock.Setup(r => r.IsVoidedAsync(_rd2Id, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is voided!"));
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(_rd2Id.ToString()));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementsNotUnique()
        {
            var command = new UpdateRequirementsCommand("", "",
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rd1Id, 1),
                    new RequirementForCommand(_rd1Id, 1)
                },
                _rowVersion);
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definitions must be unique!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_When2ErrorsWithinSameRule()
        {
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd2Id, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.IsVoidedAsync(_rd2Id, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new UpdateRequirementsCommand(
                "",
                "",
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rd1Id, 1),
                    new RequirementForCommand(_rd2Id, 1)
                },
                invalidRowVersion);

            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid RowVersion!"));
        }
    }
}
