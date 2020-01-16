using Equinor.Procosys.Preservation.Command.ModeCommands.DeleteMode;
using Equinor.Procosys.Preservation.Command.Validators.Mode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.DeleteMode
{
    [TestClass]
    public class DeleteModeCommandValidatorTests
    {
        private DeleteModeCommandValidator _dut;
        private Mock<IModeValidator> _modeValidatorMock;
        private DeleteModeCommand _command;

        private int _id = 1;

        [TestInitialize]
        public void Setup_OkState()
        {
            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.Exists(_id)).Returns(true);
            _command = new DeleteModeCommand(_id);

            _dut = new DeleteModeCommandValidator(_modeValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenModeNotExists()
        {
            _modeValidatorMock.Setup(r => r.Exists(_id)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenModeNotVoided()
        {
            _modeValidatorMock.Setup(r => r.IsVoided(_id)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is not voided!"));
        }
    }
}
