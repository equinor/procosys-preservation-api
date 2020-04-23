using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.UpdateJourney;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UpdateJourney
{
    [TestClass]
    public class UpdateJourneyCommandValidatorTests
    {
        private UpdateJourneyCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private UpdateJourneyCommand _command;

        private int _id = 1;
        private string _title = "Title";

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(true));
            _command = new UpdateJourneyCommand(_id, _title);

            _dut = new UpdateJourneyCommandValidator(_journeyValidatorMock.Object);
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
        public void Validate_ShouldFail_WhenAnotherJourneyWithSameTitleAlreadyExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsWithSameTitleInAnotherJourneyAsync(_id, _title, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Another journey with this title already exists!"));
        }
    }
}
