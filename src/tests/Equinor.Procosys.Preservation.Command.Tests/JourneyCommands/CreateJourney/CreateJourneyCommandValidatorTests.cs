using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.CreateJourney
{
    [TestClass]
    public class CreateJourneyCommandValidatorTests
    {
        private CreateJourneyCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private CreateJourneyCommand _command;

        private string _title = "Title";

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_title, default)).Returns(Task.FromResult(false));
            _command = new CreateJourneyCommand(_title);

            _dut = new CreateJourneyCommandValidator(_journeyValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenJourneyWithSameTitleAlreadyExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_title, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey with title already exists!"));
        }
    }
}
