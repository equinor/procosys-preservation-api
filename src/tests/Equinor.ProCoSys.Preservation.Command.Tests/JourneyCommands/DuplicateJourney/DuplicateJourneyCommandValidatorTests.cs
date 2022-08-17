using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.DuplicateJourney;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.DuplicateJourney
{
    [TestClass]
    public class DuplicateJourneyCommandValidatorTests
    {
        private DuplicateJourneyCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private DuplicateJourneyCommand _command;

        private int _id = 66;

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _command = new DuplicateJourneyCommand(_id);

            _journeyValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(true));
            _dut = new DuplicateJourneyCommandValidator(_journeyValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenJourneyNotExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_id, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenJourneyWithCopyTitleAlreadyExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsWithDuplicateTitleAsync(_id, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey with title for the copy already exists!"));
        }
    }
}
