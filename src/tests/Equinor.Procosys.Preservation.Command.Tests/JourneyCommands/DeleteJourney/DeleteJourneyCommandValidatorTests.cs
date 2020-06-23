using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.DeleteJourney;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.DeleteJourney
{
    [TestClass]
    public class DeleteJourneyCommandValidatorTests
    {
        private DeleteJourneyCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private DeleteJourneyCommand _command;

        private int _id = 1;

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(true));
            _journeyValidatorMock.Setup(r => r.IsVoidedAsync(_id, default)).Returns(Task.FromResult(true));
            _command = new DeleteJourneyCommand(_id, null);

            _dut = new DeleteJourneyCommandValidator(_journeyValidatorMock.Object);
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
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenJourneyNotVoided()
        {
            _journeyValidatorMock.Setup(r => r.IsVoidedAsync(_id, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey is not voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenJourneyIsInUse()
        {
            _journeyValidatorMock.Setup(r => r.IsInUseAsync(_id, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey is used!"));
        }
    }
}
