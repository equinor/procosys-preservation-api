using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.CreateStep
{
    [TestClass]
    public class CreateStepCommandValidatorTests
    {
        private CreateStepCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private Mock<IModeValidator> _modeValidatorMock;
        private Mock<IResponsibleValidator> _responsibleValidatorMock;
        private CreateStepCommand _command;

        private string _stepTitle = "S";
        private int _journeyId = 1;
        private int _modeId = 2;
        private string _responsibleCode = "B";

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(true));
            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(true));
            _responsibleValidatorMock = new Mock<IResponsibleValidator>();
            _command = new CreateStepCommand(_journeyId, _stepTitle, _modeId, _responsibleCode, AutoTransferMethod.None);

            _dut = new CreateStepCommandValidator(_journeyValidatorMock.Object, _modeValidatorMock.Object, _responsibleValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenJourneyNotExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenModeNotExists()
        {
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenJourneyIsVoided()
        {
            _journeyValidatorMock.Setup(r => r.IsVoidedAsync(_journeyId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenModeIsVoided()
        {
            _modeValidatorMock.Setup(r => r.IsVoidedAsync(_modeId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenResponsibleExistsAndIsVoided()
        {
            _responsibleValidatorMock.Setup(r => r.ExistsAndIsVoidedAsync(_responsibleCode, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Responsible is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepExistsWithTitle()
        {
            _journeyValidatorMock.Setup(r => r.AnyStepExistsWithSameTitleAsync(_journeyId, _stepTitle, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step with title already exists in journey!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(false));
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(false));
            _responsibleValidatorMock.Setup(r => r.ExistsAndIsVoidedAsync(_responsibleCode, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSupplierStepIsNotAddedToTop()
        {
            _journeyValidatorMock.Setup(r => r.HasAnyStepsAsync(_journeyId, default)).Returns(Task.FromResult(true));
            _modeValidatorMock.Setup(r => r.IsForSupplierAsync(_modeId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Supplier step can only be chosen as the first step!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenSupplierStepIsAddedToTop()
        {
            _journeyValidatorMock.Setup(r => r.HasAnyStepsAsync(_journeyId, default)).Returns(Task.FromResult(false));
            _modeValidatorMock.Setup(r => r.IsForSupplierAsync(_modeId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSettingSameAutoTransferMethod_AsAExistingStep()
        {
            var autoTransferMethod = AutoTransferMethod.OnRfccSign;
            _journeyValidatorMock.Setup(r => r.HasAnyStepWithAutoTransferMethodAsync(_journeyId, autoTransferMethod, default)).Returns(Task.FromResult(true));
            
            _command = new CreateStepCommand(_journeyId, _stepTitle, _modeId, _responsibleCode, autoTransferMethod);
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Same auto transfer method can not be set on multiple steps in a journey!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenSettingNoneAutoTransferMethod_AndAExistingStepHasNone()
        {
            var autoTransferMethod = AutoTransferMethod.None;
            _journeyValidatorMock.Setup(r => r.HasAnyStepWithAutoTransferMethodAsync(_journeyId, autoTransferMethod, default)).Returns(Task.FromResult(true));
            
            _command = new CreateStepCommand(_journeyId, _stepTitle, _modeId, _responsibleCode, autoTransferMethod);
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }
    }
}
