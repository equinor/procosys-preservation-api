using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Transfer;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Transfer
{
    [TestClass]
    public class TransferCommandValidatorTests
    {
        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private TransferCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private TransferCommand _command;

        private List<int> _tagIds;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagIds = new List<int> {TagId1, TagId2};
            _projectValidatorMock = new Mock<IProjectValidator>();
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.Exists(TagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.Exists(TagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirement(TagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirement(TagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(TagId1, PreservationStatus.Active)).Returns(true);
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(TagId2, PreservationStatus.Active)).Returns(true);
            _tagValidatorMock.Setup(r => r.HaveNextStep(TagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.HaveNextStep(TagId2)).Returns(true);
            _command = new TransferCommand(_tagIds);

            _dut = new TransferCommandValidator(_projectValidatorMock.Object, _tagValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public void Validate_ShouldFail_WhenNoTagsGiven()
        {
            var command = new TransferCommand(new List<int>());
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 tag must be given!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagsNotUnique()
        {
            var command = new TransferCommand(new List<int>{1, 1});
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be unique!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.Exists(TagId2)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoided(TagId1)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForAnyTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId1, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenPreservationIsNotActiveForAnyTag()
        {
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(TagId1, PreservationStatus.Active)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Tag must have status {PreservationStatus.Active} to transfer!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagHasNoNextStep()
        {
            _tagValidatorMock.Setup(r => r.HaveNextStep(TagId1)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't have a next step to transfer to!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_When2Errors()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.Exists(TagId2)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }
    }
}
