using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementType;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementType
{
    [TestClass]
    public class CreateRequirementTypeCommandValidatorTests
    {
        private CreateRequirementTypeCommandValidator _dut;
        private Mock<IRequirementTypeValidator> _requirementTypeValidatorMock;
        private CreateRequirementTypeCommand _command;

        private int _sortKey = 10;
        private string _title = "Title";
        private string _code = "Code";

        [TestInitialize]
        public void Setup_OkState()
        {
            _requirementTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _command = new CreateRequirementTypeCommand(_sortKey, _code, _title);
            _dut = new CreateRequirementTypeCommandValidator(_requirementTypeValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeWithSameTitleAlreadyExists()
        {
            _requirementTypeValidatorMock.Setup(r => r.IsNotUniqueTitleAsync(_title, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type with this title already exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementTypeWithSameCodeAlreadyExists()
        {
            _requirementTypeValidatorMock.Setup(r => r.IsNotUniqueCodeAsync(_code, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type with this code already exists!"));
        }
    }
}
