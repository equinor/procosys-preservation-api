using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ModeCommands.UpdateMode;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.UpdateMode
{
    [TestClass]
    public class UpdateModeCommandValidatorTests
    {
        private UpdateModeCommandValidator _dut;
        private Mock<IModeValidator> _modeValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UpdateModeCommand _command;

        private int _id = 1;
        private string _title = "Title";
        private bool _forSupplier = true;
        private readonly string _rowVersion = "AAAAAAAAJ00=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(_rowVersion, default)).Returns(Task.FromResult(true));

            _command = new UpdateModeCommand(_id, _title, _forSupplier, _rowVersion);

            _dut = new UpdateModeCommandValidator(_modeValidatorMock.Object, _rowVersionValidatorMock.Object);
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
            _modeValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnotherModeWithSameTitleAlreadyExists()
        {
            _modeValidatorMock.Setup(r => r.ExistsAnotherModeWithSameTitleAsync(_id, _title, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode with title already exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenModeIsVoided()
        {
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_id, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnotherModeForSupplierAlreadyExists_AndUpdatingModeToForSupplier()
        {
            _modeValidatorMock.Setup(r => r.ExistsAnotherModeForSupplierAsync(_id, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Another mode for supplier already exists!"));
        }

        [TestMethod]
        public void Validate_ShouldBeOk_WhenAnotherModeForSupplierAlreadyExists_AndNotUpdatingModeToForSupplier()
        {
            _modeValidatorMock.Setup(r => r.ExistsAnotherModeForSupplierAsync(_id, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new UpdateModeCommand(_id, _title, _forSupplier, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid RowVersion!"));
        }
    }
}
