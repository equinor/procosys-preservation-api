using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CompletePreservation;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.CompletePreservation
{
    [TestClass]
    public class CompletePreservationCommandValidatorTests
    {
        private CompletePreservationCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private CompletePreservationCommand _command;

        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private const string RowVersion1 = "AAAAAAAAABA=";
        private const string RowVersion2 = "AAAAAAAABBA=";

        private List<int> _tagIds;
        private List<IdAndRowVersion> _tagsIdsWithRowVersion;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagIds = new List<int> { TagId1, TagId2 };
            _tagsIdsWithRowVersion = new List<IdAndRowVersion>
            {
                new IdAndRowVersion(TagId1, RowVersion1),
                new IdAndRowVersion(TagId2, RowVersion2)
            };
            _projectValidatorMock = new Mock<IProjectValidator>();
            _projectValidatorMock.Setup(p => p.AllTagsInSameProjectAsync(_tagIds, default)).Returns(Task.FromResult(true));
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId2, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToBeCompletedAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToBeCompletedAsync(TagId2, default)).Returns(Task.FromResult(true));
            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion1)).Returns(true);
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion2)).Returns(true);
            _command = new CompletePreservationCommand(_tagsIdsWithRowVersion);

            _dut = new CompletePreservationCommandValidator(
                _projectValidatorMock.Object,
                _tagValidatorMock.Object,
                _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenNoTagsGiven()
        {
            var command = new CompletePreservationCommand(new List<IdAndRowVersion>());

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 tag must be given!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagsNotUnique()
        {
            var command = new CompletePreservationCommand(new List<IdAndRowVersion> { new IdAndRowVersion(1, null), new IdAndRowVersion(1, null) });

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be unique!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnyTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId2, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnyTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(TagId1, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenProjectForFirstTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(_tagIds.First(), default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagsInDifferentProjects()
        {
            _projectValidatorMock.Setup(r => r.AllTagsInSameProjectAsync(_tagIds, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be in same project!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenNotReadyToBeCompleted()
        {
            _tagValidatorMock.Setup(r => r.IsReadyToBeCompletedAsync(TagId1, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Preservation on tag can not be completed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _tagValidatorMock.Setup(r => r.VerifyPreservationStatusAsync(TagId1, PreservationStatus.NotStarted, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId2, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFailWith1Error_WhenErrorsInDifferentRules()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId2, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion2)).Returns(false);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }

        [TestMethod]
        public async Task Validate_ShouldIncludeTagNoInMessage()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(TagId1, default)).Returns(Task.FromResult(true));
            var tagNo = "Tag X";
            _tagValidatorMock.Setup(r => r.GetTagDetailsAsync(TagId1, default)).Returns(Task.FromResult(tagNo));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(tagNo));
        }
    }
}
