using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.VoidJourney;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.VoidJourney
{
    [TestClass]
    public class VoidJourneyCommandValidatorTests
    {
        private Mock<IJourneyValidator> _journeyValidatorMock;

        private VoidJourneyCommand _command;
        private VoidJourneyCommandValidator _dut;
        private readonly int _journeyId = 2;

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(true));

            _command = new VoidJourneyCommand(_journeyId);
            _dut = new VoidJourneyCommandValidator(_journeyValidatorMock.Object);
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
        public void Validate_ShouldFail_WhenJourneyIsVoided()
        {
            _journeyValidatorMock.Setup(r => r.IsVoidedAsync(_journeyId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey is voided!"));
        }
    }
}
