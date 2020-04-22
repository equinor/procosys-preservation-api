using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateStep;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateStep
{
    [TestClass]
    public class UpdateStepCommandValidatorTests
    {
        private UpdateStepCommandValidator _dut;
        private Mock<IStepValidator> _stepValidatorMock;
        private UpdateStepCommand _command;

        private int _id = 1;
        private string _title = "Title";

        [TestInitialize]
        public void Setup_OkState()
        {
            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(false));

            _command = new UpdateStepCommand(stepId:_id, _title);

            _dut = new UpdateStepCommandValidator(_stepValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepNotExists()
        {
            _stepValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnotherStepWithSameTitleAlreadyExists()
        {
            _stepValidatorMock.Setup(r => r.ExistsInJourneyAsync(_id, _title, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Another step with title already exists in journey!"));
        }
    }
}
