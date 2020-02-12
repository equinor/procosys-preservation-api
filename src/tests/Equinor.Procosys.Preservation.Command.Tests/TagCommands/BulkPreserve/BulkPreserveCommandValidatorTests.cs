using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.BulkPreserve
{
    [TestClass]
    public class BulkPreserveCommandValidatorTests
    {
        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private DateTime _utcNow;
        private BulkPreserveCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<ITimeService> _timeServiceMock;
        private BulkPreserveCommand _command;

        private List<int> _tagIds;

        [TestInitialize]
        public void Setup_OkState()
        {
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _tagIds = new List<int> {TagId1, TagId2};
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.Exists(TagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.Exists(TagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirement(TagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.HasANonVoidedRequirement(TagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(TagId1, PreservationStatus.Active)).Returns(true);
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatus(TagId2, PreservationStatus.Active)).Returns(true);
            _tagValidatorMock.Setup(r => r.ReadyToBePreserved(TagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.ReadyToBePreserved(TagId2)).Returns(true);
            _tagValidatorMock.Setup(r => r.ReadyToBeBulkPreserved(TagId1, _utcNow)).Returns(true);
            _tagValidatorMock.Setup(r => r.ReadyToBeBulkPreserved(TagId2, _utcNow)).Returns(true);
            _timeServiceMock = new Mock<ITimeService>();
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_utcNow);
            _command = new BulkPreserveCommand(_tagIds);

            _dut = new BulkPreserveCommandValidator(_tagValidatorMock.Object, _timeServiceMock.Object);
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
            var command = new BulkPreserveCommand(new List<int>());
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 tag must be given!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagsNotUnique()
        {
            var command = new BulkPreserveCommand(new List<int>{1, 1});
            
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
            _tagValidatorMock.Setup(r => r.ProjectIsClosed(TagId1)).Returns(true);
            
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
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Tag must have status {PreservationStatus.Active} to preserve!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotReadyToBePreserved()
        {
            _tagValidatorMock.Setup(r => r.ReadyToBePreserved(TagId1)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is not ready to be preserved!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotReadyToBeBulkPreserved()
        {
            _tagValidatorMock.Setup(r => r.ReadyToBeBulkPreserved(TagId1, _utcNow)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is not ready to be bulk preserved!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_When2Errors()
        {
            _tagValidatorMock.Setup(r => r.ProjectIsClosed(TagId1)).Returns(true);
            _tagValidatorMock.Setup(r => r.Exists(TagId2)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exists!"));
        }
    }
}
