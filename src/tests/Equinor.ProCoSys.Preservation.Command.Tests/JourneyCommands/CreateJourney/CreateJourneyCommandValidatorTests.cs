﻿using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.JourneyCommands.CreateJourney;
using Equinor.ProCoSys.Preservation.Command.Validators.JourneyValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.JourneyCommands.CreateJourney
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
            _command = new CreateJourneyCommand(_title);

            _dut = new CreateJourneyCommandValidator(_journeyValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenJourneyWithSameTitleAlreadyExists()
        {
            _journeyValidatorMock.Setup(r => r.ExistsWithSameTitleAsync(_title, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey with title already exists!"));
        }
    }
}
