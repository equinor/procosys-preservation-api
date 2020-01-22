using Equinor.Procosys.Preservation.Command.TagCommands.SetStep;
using Equinor.Procosys.Preservation.Command.Validators.Step;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.SetStep
{
    [TestClass]
    public class SetStepCommandValidatorTests
    {
        private SetStepCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private SetStepCommand _command;

        private int _tagId = 1;
        private int _stepId = 2;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.Exists(_tagId)).Returns(true);
            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.Exists(_stepId)).Returns(true);
            _command = new SetStepCommand(_tagId, _stepId);

            _dut = new SetStepCommandValidator(
                _tagValidatorMock.Object, 
                _stepValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.Exists(_tagId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepNotExists()
        {
            _stepValidatorMock.Setup(r => r.Exists(_stepId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoided(_tagId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepIsVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoided(_stepId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _tagValidatorMock.Setup(r => r.ProjectIsClosed(_tagId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith2Errors_WhenErrorsInDifferentRules()
        {
            _tagValidatorMock.Setup(r => r.Exists(_tagId)).Returns(false);
            _stepValidatorMock.Setup(r => r.Exists(_stepId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
        }
    }
}
