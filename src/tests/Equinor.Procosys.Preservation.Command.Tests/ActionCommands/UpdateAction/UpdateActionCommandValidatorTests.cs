using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction;
using Equinor.Procosys.Preservation.Command.Validators.ActionValidators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionCommands.UpdateAction
{
    [TestClass]
    public class UpdateActionCommandValidatorTests
    {
        private UpdateActionCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IActionValidator> _actionValidatorMock;
        private UpdateActionCommand _command;

        private readonly int _tagId = 2;
        private readonly int _actionId = 3;

        [TestInitialize]
        public void Setup_OkState()
        {
            _projectValidatorMock = new Mock<IProjectValidator>();

            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsAsync(_tagId, default)).Returns(Task.FromResult(true));

            _actionValidatorMock = new Mock<IActionValidator>();
            _actionValidatorMock.Setup(r => r.ExistsAsync(_actionId, default)).Returns(Task.FromResult(true));

            _command = new UpdateActionCommand(
                _tagId,
                _actionId,
                "",
                "",
                null,
                null);

            _dut = new UpdateActionCommandValidator(
                _projectValidatorMock.Object,
                _tagValidatorMock.Object,
                _actionValidatorMock.Object
                );
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(_tagId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_tagId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(_tagId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenActionIsClosed()
        {
            _actionValidatorMock.Setup(r => r.IsClosedAsync(_actionId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Action is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenActionNotExists()
        {
            _actionValidatorMock.Setup(r => r.ExistsAsync(_actionId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Action doesn't exist!"));
        }
    }
}
