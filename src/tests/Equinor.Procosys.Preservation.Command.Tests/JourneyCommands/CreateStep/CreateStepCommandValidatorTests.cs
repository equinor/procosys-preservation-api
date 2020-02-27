﻿using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.JourneyCommands.CreateStep;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Command.Validators.Mode;
using Equinor.Procosys.Preservation.Command.Validators.Responsible;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.CreateStep
{
    [TestClass]
    public class CreateStepCommandValidatorTests
    {
        private CreateStepCommandValidator _dut;
        private Mock<IJourneyValidator> _journeyValidatorMock;
        private Mock<IModeValidator> _modeValidatorMock;
        private Mock<IResponsibleValidator> _responsibleValidatorMock;
        private CreateStepCommand _command;

        private int _journeyId = 1;
        private int _modeId = 2;
        private int _responsibleId = 3;

        [TestInitialize]
        public void Setup_OkState()
        {
            _journeyValidatorMock = new Mock<IJourneyValidator>();
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(true));
            _modeValidatorMock = new Mock<IModeValidator>();
            _modeValidatorMock.Setup(r => r.Exists(_modeId)).Returns(true);
            _responsibleValidatorMock = new Mock<IResponsibleValidator>();
            _responsibleValidatorMock.Setup(r => r.Exists(_responsibleId)).Returns(true);
            _command = new CreateStepCommand(_journeyId, _modeId, _responsibleId);

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
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Journey doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenModeNotExists()
        {
            _modeValidatorMock.Setup(r => r.Exists(_modeId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenResponsibleNotExists()
        {
            _responsibleValidatorMock.Setup(r => r.Exists(_responsibleId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Responsible doesn't exists!"));
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
            _modeValidatorMock.Setup(r => r.IsVoided(_modeId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Mode is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenResponsibleIsVoided()
        {
            _responsibleValidatorMock.Setup(r => r.IsVoided(_responsibleId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Responsible is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith3Errors_WhenErrorsInAllRules()
        {
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(false));
            _modeValidatorMock.Setup(r => r.Exists(_modeId)).Returns(false);
            _responsibleValidatorMock.Setup(r => r.Exists(_responsibleId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(3, result.Errors.Count);
        }

        [TestMethod]
        public void Validate_ShouldFailWith3Errors_WhenAllValidatorsFail()
        {
            _journeyValidatorMock.Setup(r => r.ExistsAsync(_journeyId, default)).Returns(Task.FromResult(false));
            _modeValidatorMock.Setup(r => r.Exists(_modeId)).Returns(false);
            _responsibleValidatorMock.Setup(r => r.Exists(_responsibleId)).Returns(false);

            _journeyValidatorMock.Setup(r => r.IsVoidedAsync(_journeyId, default)).Returns(Task.FromResult(true));
            _modeValidatorMock.Setup(r => r.IsVoided(_modeId)).Returns(true);
            _responsibleValidatorMock.Setup(r => r.IsVoided(_responsibleId)).Returns(true);

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(3, result.Errors.Count);
        }
    }
}
