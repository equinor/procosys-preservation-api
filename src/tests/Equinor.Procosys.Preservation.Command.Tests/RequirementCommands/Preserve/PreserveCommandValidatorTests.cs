using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementCommands.Preserve
{
    [TestClass]
    public class PreserveCommandValidatorTests
    {
        private const int TagId = 7;
        private const int RequirementId = 71;
        private PreserveCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private PreserveCommand _command;

        [TestInitialize]
        public void Setup_OkState()
        {
            _projectValidatorMock = new Mock<IProjectValidator>();
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsRequirementAsync(TagId, RequirementId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirementAsync(TagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatusAsync(TagId, PreservationStatus.Active, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.RequirementIsReadyToBePreservedAsync(TagId, RequirementId, default)).Returns(Task.FromResult(true));
            _command = new PreserveCommand(TagId, RequirementId);

            _dut = new PreserveCommandValidator(_projectValidatorMock.Object, _tagValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagOrReqNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsRequirementAsync(TagId, RequirementId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag and/or requirement doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(TagId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenPreservationIsNotActiveForTag()
        {
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatusAsync(TagId, PreservationStatus.Active, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Tag must have status {PreservationStatus.Active} to preserve!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementNotReadyToBePreserved()
        {
            _tagValidatorMock.Setup(r => r.RequirementIsReadyToBePreservedAsync(TagId, RequirementId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't have this requirement ready to be preserved!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }
    }
}
