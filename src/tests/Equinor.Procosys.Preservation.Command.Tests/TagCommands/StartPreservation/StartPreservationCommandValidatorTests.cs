using System.Collections.Generic;
using Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.StartPreservation
{
    [TestClass]
    public class StartPreservationCommandValidatorTests
    {
        private StartPreservationCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private StartPreservationCommand _command;

        private int _tagId1 = 7;
        private int _tagId2 = 8;
        private List<int> _tagIds;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagIds = new List<int> {_tagId1, _tagId2};
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.Exists(_tagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.Exists(_tagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirement(_tagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirement(_tagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(_tagId1, PreservationStatus.NotStarted)).Returns(true);
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(_tagId2, PreservationStatus.NotStarted)).Returns(true);
            _command = new StartPreservationCommand(_tagIds);

            _dut = new StartPreservationCommandValidator(_tagValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.Exists(_tagId2)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoided(_tagId1)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForAnyTagIsClosed()
        {
            _tagValidatorMock.Setup(r => r.ProjectIsClosed(_tagId1)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagMissesNonVoidedRequirement()
        {
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirement(_tagId1)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag do not have any non voided requirement!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenPreservationIsStartedForAnyTag()
        {
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(_tagId1, PreservationStatus.NotStarted)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Preservation is already started!"));
        }
    }
}
