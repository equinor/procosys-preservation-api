using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve.BulkPreserve;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Preserve.BulkPreserve
{
    [TestClass]
    public class BulkPreserveCommandValidatorTests
    {
        private DateTime _utcNow;
        private BulkPreserveCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<ITimeService> _timeServiceMock;
        private BulkPreserveCommand _command;

        private int _tagId1 = 7;
        private int _tagId2 = 8;
        private List<int> _tagIds;

        [TestInitialize]
        public void Setup_OkState()
        {
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _tagIds = new List<int> {_tagId1, _tagId2};
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.Exists(_tagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.Exists(_tagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirement(_tagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirement(_tagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(_tagId1, PreservationStatus.Active)).Returns(true);
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(_tagId2, PreservationStatus.Active)).Returns(true);
            _tagValidatorMock.Setup(r => r.ReadyToBePreserved(_tagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.ReadyToBePreserved(_tagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.ReadyToBeBulkPreserved(_tagId1, _utcNow)).Returns(true);
            _tagValidatorMock.Setup(r => r.ReadyToBeBulkPreserved(_tagId2, _utcNow)).Returns(true);
            _timeServiceMock = new Mock<ITimeService>();
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_utcNow);
            _command = new BulkPreserveCommand(_tagIds);

            _dut = new BulkPreserveCommandValidator(
                new PreserveCommandValidator(_tagValidatorMock.Object), 
                _tagValidatorMock.Object, _timeServiceMock.Object);
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
            var command = new PreserveCommand(new List<int>());
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 tag must be given!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagsNotUnique()
        {
            var command = new PreserveCommand(new List<int>{1, 1});
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be unique!"));
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
        public void Validate_ShouldFail_WhenPreservationIsNotActiveForAnyTag()
        {
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(_tagId1, PreservationStatus.Active)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Tag must have status {PreservationStatus.Active} to preserve!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotReadyToBePreserved()
        {
            _tagValidatorMock.Setup(r => r.ReadyToBePreserved(_tagId1)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is not ready to be preserved!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotReadyToBeBulkPreserved()
        {
            _tagValidatorMock.Setup(r => r.ReadyToBeBulkPreserved(_tagId1, _utcNow)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is not ready to be bulk preserved!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_When2Errors()
        {
            _tagValidatorMock.Setup(r => r.ProjectIsClosed(_tagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.Exists(_tagId2)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exists!"));
        }
    }
}
