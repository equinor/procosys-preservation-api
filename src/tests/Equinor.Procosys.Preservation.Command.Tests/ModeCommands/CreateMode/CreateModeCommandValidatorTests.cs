using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.CreateMode;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.CreateMode
{
    [TestClass]
    public class CreateModeCommandValidatorTests
    {
        private CreateModeCommandValidator _dut;
        private Mock<IModeValidator> _modeValidatorMock;
        private CreateModeCommand _command;

        private string _title = "Title";
        private bool _forSupplier = true;

        [TestInitialize]
        public void Setup_OkState()
        {
            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.ExistsWithSameTitleAsync(_title, default)).Returns(Task.FromResult(false));
            _modeValidatorMock.Setup(r => r.ExistsAnotherModeTitleForSupplierAsync(_title, _forSupplier, default)).Returns(Task.FromResult(false));
            _command = new CreateModeCommand(_title, _forSupplier);

            _dut = new CreateModeCommandValidator(_modeValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenModeAlreadyExists()
        {
            _modeValidatorMock.Setup(r => r.ExistsWithSameTitleAsync(_title, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode with title already exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnotherModeForSupplierAlreadyExists()
        {
            _modeValidatorMock.Setup(r => r.ExistsAnotherModeTitleForSupplierAsync(_title, _forSupplier, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Another mode for supplier already exists!"));
        }
    }
}
