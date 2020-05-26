using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using Equinor.Procosys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandValidatorTests
    {
        private UpdateStepCommandValidator _dut;
        private UpdateStepCommand _command;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IModeValidator> _modeValidatorMock;
        private Mock<IResponsibleValidator> _responsibleValidatorMock;

        private int _journeyId = 1;
        private int _modeId = 2;
        private int _stepId = 3;
        private string _responsibleCode = "B";
        private string _title = "T";

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(true));
            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.ExistsAsync(_modeId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(s => s.ExistsAsync(_stepId, default)).Returns(Task.FromResult(true));
            _responsibleValidatorMock = new Mock<IResponsibleValidator>();
            _responsibleValidatorMock.Setup(r => r.ExistsAndIsVoidedAsync(_responsibleCode, default)).Returns(Task.FromResult(false));
            _command = new UpdateStepCommand(_journeyId, _stepId, _modeId, _responsibleCode, _title, null);

            _dut = new UpdateStepCommandValidator(_journeyValidatorMock.Object, _stepValidatorMock.Object, _modeValidatorMock.Object, _responsibleValidatorMock.Object);
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
    }
}
